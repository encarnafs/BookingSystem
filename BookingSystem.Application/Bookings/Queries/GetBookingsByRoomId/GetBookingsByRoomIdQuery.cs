using BookingSystem.Application.Bookings.Dtos;
using MediatR;

namespace BookingSystem.Application.Bookings.Queries.GetBookingsByRoomId;

public record GetBookingsByRoomIdQuery(Guid RoomId)
    : IRequest<List<BookingDto>>;
