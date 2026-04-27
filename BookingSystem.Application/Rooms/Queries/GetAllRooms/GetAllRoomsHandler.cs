using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Rooms.Dtos;
using MediatR;

namespace BookingSystem.Application.Rooms.Queries.GetAllRooms;

public class GetAllRoomsHandler : IRequestHandler<GetAllRoomsQuery, IReadOnlyList<RoomDto>>
{
    private readonly IRoomRepository _roomRepository;

    public GetAllRoomsHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IReadOnlyList<RoomDto>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository .GetAllAsync(cancellationToken);

        return rooms
            .Select(room => new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Description = room.Description ?? string.Empty,
                IsActive = room.IsActive
            })
            .ToList();
    }
}
