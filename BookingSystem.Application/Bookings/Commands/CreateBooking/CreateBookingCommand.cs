using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid RoomId,
    Guid ClientId,
    Guid CreatedByUserId,
    DateTime Start,
    DateTime End,
    string? Comments
) : IRequest<BookingDto>;