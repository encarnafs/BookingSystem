using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Users.Events;

public class UserPasswordChangedHandler : INotificationHandler<UserPasswordChangedNotification>
{
    private readonly IAuditService _auditService;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserPasswordChangedHandler> _logger;

    public UserPasswordChangedHandler(
        IAuditService auditService,
        IEmailService emailService,
        ILogger<UserPasswordChangedHandler> logger)
    {
        _auditService = auditService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(UserPasswordChangedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Auditando cambio de contraseña del usuario {UserId}", notification.UserId);

        await _auditService.AddEntryAsync(
            userId: notification.UserId,
            action: "UserPasswordChanged",
            entity: "User",
            entityId: notification.UserId,
            oldValues: new { Password = "********" },
            newValues: new { Password = "********" });

        await _emailService.SendAsync(
            to: "admin@bookingsystem.com",
            subject: "Cambio de contraseña detectado",
            body: $"El usuario con ID {notification.UserId} ha cambiado su contraseña.");
    }
}
