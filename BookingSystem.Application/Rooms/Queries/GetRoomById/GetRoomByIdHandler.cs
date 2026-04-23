using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Rooms.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdHandler : IRequestHandler<GetRoomByIdQuery, RoomDto>
{
    private readonly IRoomRepository _roomRepository;
    public GetRoomByIdHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    public async Task<RoomDto> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
        if (room == null)
            throw new NotFoundException($"Sala con Id {request.Id} no encontrada.", request.Id);

        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Description = room.Description ?? string.Empty,
            IsActive = room.IsActive
        };
    }
}
