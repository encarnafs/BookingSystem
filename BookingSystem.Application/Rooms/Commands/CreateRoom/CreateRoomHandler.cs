using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Rooms.Commands.CreateRoom;
using BookingSystem.Domain.Entities;
using MediatR;

public class CreateRoomHandler : IRequestHandler<CreateRoomCommand, Guid>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoomHandler(
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = new Room(
            request.Name,
            request.Capacity,
            request.Description
        );

        await _roomRepository.AddAsync(room, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return room.Id;
    }
}
