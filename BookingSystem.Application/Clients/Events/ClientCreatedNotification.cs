using MediatR;

namespace BookingSystem.Application.Clients.Events;

public record ClientCreatedNotification(Guid ClientId) : INotification;
