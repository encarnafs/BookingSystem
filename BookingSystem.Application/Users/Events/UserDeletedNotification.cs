using MediatR;

namespace BookingSystem.Application.Users.Events;

public record UserDeletedNotification(Guid UserId) : INotification;
