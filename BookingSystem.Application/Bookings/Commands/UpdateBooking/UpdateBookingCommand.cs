using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.UpdateBooking;
public  record UpdateBookingCommand(
    Guid Id,
    Guid RoomId,
    Guid ClientId,
    DateRange DateRange,
    string? Comments
) : IRequest<Unit>;
