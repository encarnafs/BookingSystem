using MediatR;

namespace BookingSystem.Application.Users.Events;
public record UserDisabledNotification(Guid UserId) : INotification;
