using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomHandler : IRequestHandler<UpdateRoomCommand>
{
    private readonly IRoomRepository _roomRepository;

    public UpdateRoomHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);

        if (room is null)
            throw new NotFoundException("Room", request.Id);

        // 🔹 Actualizar nombre
        room.UpdateName(request.Name);

        // 🔹 Actualizar capacidad
        room.UpdateCapacity(request.Capacity);

        // 🔹 Actualizar descripción
        room.UpdateDescription(request.Description);

        // 🔹 Activar / desactivar según corresponda
        if (request.IsActive && !room.IsActive)
            room.Activate();

        if (!request.IsActive && room.IsActive)
            room.Deactivate();

        // 🔹 Guardar cambios
        await _roomRepository.UpdateAsync(room, cancellationToken);
    }
}


