using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBookingComments;

public record UpdateBookingCommentsCommand(
    Guid Id,
    string? Comments
) : IRequest;
