using MediatR;

namespace BookingSystem.Application.Bookings.Events;

public record BookingCreatedNotification(Guid BookingId, string Email) : INotification;
