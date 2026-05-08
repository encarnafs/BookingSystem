using MediatR;

namespace BookingSystem.Application.Bookings.Events;

public record BookingUpdatedNotification(Guid BookingId) : INotification;
