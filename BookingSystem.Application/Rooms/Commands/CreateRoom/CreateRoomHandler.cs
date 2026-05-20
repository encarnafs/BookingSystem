using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Rooms.Commands.CreateRoom;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Exceptions;
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
        // 1. Comprobar si ya existe una habitación con el mismo nombre
        var existingRoom = await _roomRepository.GetByNameAsync(request.Name, cancellationToken);

        if (existingRoom is not null)
            throw new InvalidRoomNameException(request.Name);

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
