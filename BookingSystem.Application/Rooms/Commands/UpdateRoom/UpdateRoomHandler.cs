using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Exceptions;
using MediatR;

namespace BookingSystem.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomHandler : IRequestHandler<UpdateRoomCommand>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomHandler(
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room", request.Id);

        // Checkea si ya existe una sala con el mismo nombre
        var existingRoom = await _roomRepository.GetByNameAsync(request.Name, cancellationToken);

        if (existingRoom is not null && existingRoom.Id != request.Id)
            throw new InvalidRoomNameException(request.Name);

        room.UpdateName(request.Name);
        room.UpdateCapacity(request.Capacity);
        room.UpdateDescription(request.Description);

        if (request.IsActive && !room.IsActive)
            room.Activate();

        if (!request.IsActive && room.IsActive)
            room.Deactivate();

        await _roomRepository.UpdateAsync(room, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}



