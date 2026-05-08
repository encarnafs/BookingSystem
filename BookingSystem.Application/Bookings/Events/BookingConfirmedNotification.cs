using MediatR;

namespace BookingSystem.Application.Bookings.Events;

public record BookingConfirmedNotification(Guid BookingId) : INotification;
