using MediatR;

namespace BookingSystem.Application.Bookings.Events;

public record BookingDatesUpdatedNotification(Guid BookingId, string Email) : INotification;
