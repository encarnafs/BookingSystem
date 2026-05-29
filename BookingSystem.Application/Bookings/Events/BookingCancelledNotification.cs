using MediatR;

namespace BookingSystem.Application.Bookings.Events;

public record BookingCancelledNotification(Guid BookingId, string Email) : INotification;
