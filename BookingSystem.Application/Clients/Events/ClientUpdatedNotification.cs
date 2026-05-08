using MediatR;

namespace BookingSystem.Application.Clients.Events;

public record ClientUpdatedNotification(Guid ClientId, object OldValues, object NewValues) : INotification;
