using MediatR;

namespace BookingSystem.Application.Users.Events;
public record UserUpdatedNotification(Guid UserId, object OldValues, object NewValues) : INotification;
