using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingDates;

public record UpdateBookingDatesCommand(
    Guid Id,
    DateTime Start,
    DateTime End
) : IRequest;
