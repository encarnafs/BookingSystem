using BookingSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Users.Events;

public class UserRoleChangedHandler : INotificationHandler<UserRoleChangedNotification>
{
    private readonly IAuditService _auditService;
    private readonly ILogger<UserRoleChangedHandler> _logger;

    public UserRoleChangedHandler(
        IAuditService auditService,
        ILogger<UserRoleChangedHandler> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(UserRoleChangedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Auditando cambio de rol del usuario {UserId}", notification.UserId);

        await _auditService.AddEntryAsync(
            userId: notification.UserId,
            action: "UserRoleChanged",
            entity: "User",
            entityId: notification.UserId,
            oldValues: new { Role = notification.OldRole },
            newValues: new { Role = notification.NewRole }
        );
    }
}
