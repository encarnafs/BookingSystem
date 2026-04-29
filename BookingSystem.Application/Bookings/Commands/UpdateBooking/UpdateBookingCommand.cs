using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBooking;
public  record UpdateBookingCommand(
    Guid Id,
    Guid RoomId,
    Guid ClientId,
    DateTime Start,
    DateTime End,
    string? Comments
) : IRequest<Unit>;
