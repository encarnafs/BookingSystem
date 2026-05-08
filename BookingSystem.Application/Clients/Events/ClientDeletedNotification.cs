using MediatR;

namespace BookingSystem.Application.Clients.Events;

public record ClientDeletedNotification(
    Guid ClientId,
    object OldValues,
    object NewValues
) : INotification;
