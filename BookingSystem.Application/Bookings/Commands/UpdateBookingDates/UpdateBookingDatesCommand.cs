using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingDates;

public record UpdateBookingDatesCommand(
    Guid BookingId,
    DateTime Start,
    DateTime End
) : IRequest;
