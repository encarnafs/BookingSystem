using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using MediatR;

namespace BookingSystem.Application.Rooms.Commands.CreateRoom;

public class CreateRoomHandler : IRequestHandler<CreateRoomCommand, Guid>
{
    private readonly IRoomRepository _roomRepository;

    public CreateRoomHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<Guid> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = new Room(
            request.Name,
            request.Capacity,
            request.Description
        );

        await _roomRepository.AddAsync(room, cancellationToken);

        return room.Id;
    }
}
