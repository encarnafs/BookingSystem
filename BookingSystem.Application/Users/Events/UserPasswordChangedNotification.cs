using MediatR;

namespace BookingSystem.Application.Users.Events;
public record UserPasswordChangedNotification(Guid UserId) : INotification;
