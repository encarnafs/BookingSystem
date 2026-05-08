using BookingSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Users.Events;

public class UserDeletedHandler : INotificationHandler<UserDeletedNotification>
{
    private readonly IAuditService _auditService;
    private readonly ILogger<UserDeletedHandler> _logger;

    public UserDeletedHandler(
        IAuditService auditService,
        ILogger<UserDeletedHandler> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Auditando eliminación del usuario {UserId}", notification.UserId);

        await _auditService.AddEntryAsync(
            userId: notification.UserId,
            action: "UserDeleted",
            entity: "User",
            entityId: notification.UserId,
            oldValues: null,
            newValues: new { DeletedAt = DateTime.UtcNow });
    }
}
