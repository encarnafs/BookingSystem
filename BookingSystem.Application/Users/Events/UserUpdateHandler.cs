using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserUpdatedHandler : INotificationHandler<UserUpdatedNotification>
{
    private readonly IAuditService _auditService;
    private readonly ILogger<UserUpdatedHandler> _logger;

    public UserUpdatedHandler(IAuditService auditService, ILogger<UserUpdatedHandler> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(UserUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Auditando actualización del usuario {UserId}", notification.UserId);

        await _auditService.AddEntryAsync(
            userId: notification.UserId,
            action: "UserUpdated",
            entity: "User",
            entityId: notification.UserId,
            oldValues: notification.OldValues,
            newValues: notification.NewValues);
    }
}
