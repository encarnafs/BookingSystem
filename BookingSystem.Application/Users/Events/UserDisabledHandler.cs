using BookingSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Application.Users.Events;

public class UserDisabledHandler : INotificationHandler<UserDisabledNotification>
{
    private readonly IAuditService _auditService;
    private readonly ILogger<UserDisabledHandler> _logger;

    public UserDisabledHandler(
        IAuditService auditService,
        ILogger<UserDisabledHandler> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(UserDisabledNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Auditando desactivación del usuario {UserId}", notification.UserId);

        await _auditService.AddEntryAsync(
            userId: notification.UserId,
            action: "UserDisabled",
            entity: "User",
            entityId: notification.UserId,
            oldValues: null,
            newValues: new { DisabledAt = DateTime.UtcNow });
    }
}
