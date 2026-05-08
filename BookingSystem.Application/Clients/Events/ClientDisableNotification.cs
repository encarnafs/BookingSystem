using MediatR;

namespace BookingSystem.Application.Clients.Events;

public record ClientDisabledNotification(
    Guid ClientId,
    object OldValues,
    object NewValues
) : INotification;
