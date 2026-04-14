using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingComments;

public record UpdateBookingCommentsCommand(
    Guid BookingId,
    string? Comments
) : IRequest;
