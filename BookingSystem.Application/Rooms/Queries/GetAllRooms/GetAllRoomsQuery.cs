using BookingSystem.Application.Rooms.Dtos;
using MediatR;

namespace BookingSystem.Application.Rooms.Queries.GetAllRooms;

public record GetAllRoomsQuery() : IRequest<IReadOnlyList<RoomDto>>;