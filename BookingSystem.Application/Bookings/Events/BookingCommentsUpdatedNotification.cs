using MediatR;

namespace BookingSystem.Application.Bookings.Events;

public record BookingCommentsUpdatedNotification(Guid BookingId) : INotification;
