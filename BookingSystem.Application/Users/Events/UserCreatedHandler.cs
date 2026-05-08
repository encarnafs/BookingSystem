using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Users.Events;

public class UserCreatedHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly IAuditService _auditService;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserCreatedHandler> _logger;

    public UserCreatedHandler(
        IAuditService auditService,
        IEmailService emailService,
        ILogger<UserCreatedHandler> logger)
    {
        _auditService = auditService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Auditando creación del usuario {UserId}", notification.UserId);

        await _auditService.AddEntryAsync(
            userId: notification.UserId,
            action: "UserCreated",
            entity: "User",
            entityId: notification.UserId,
            oldValues: null,
            newValues: new { Created = DateTime.UtcNow });

        await _emailService.SendAsync(
            to: "admin@bookingsystem.com",
            subject: "Nuevo usuario creado",
            body: $"Se ha creado el usuario con ID {notification.UserId}.");
    }
}
