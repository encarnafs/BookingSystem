using BookingSystem.Application.Rooms.Dtos;
using MediatR;

namespace BookingSystem.Application.Rooms.Queries.CheckAvailability;

public record CheckRoomAvailabilityQuery(
    Guid RoomId,
    DateTime Start,
    DateTime End
) : IRequest<RoomAvailabilityDto>;
