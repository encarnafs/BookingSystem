using MediatR;

namespace BookingSystem.Application.Users.Events;
public record UserRoleChangedNotification(Guid UserId, string OldRole, string NewRole) : INotification;
