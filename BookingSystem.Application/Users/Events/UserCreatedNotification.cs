using MediatR;

namespace BookingSystem.Application.Users.Events;
public record UserCreatedNotification(Guid UserId) : INotification;
