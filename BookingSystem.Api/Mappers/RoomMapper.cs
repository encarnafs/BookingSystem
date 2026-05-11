using BookingSystem.Api.Requests.Rooms;
using BookingSystem.Application.Rooms.Commands.CreateRoom;
using BookingSystem.Application.Rooms.Commands.UpdateRoom;
using BookingSystem.Application.Rooms.Dtos;
using BookingSystem.Api.Responses.Rooms;

namespace BookingSystem.Api.Mappers;

public static class RoomMapper
{
    public static CreateRoomCommand ToCommand(this CreateRoomRequest request)
    {
        return new CreateRoomCommand(
            request.Name,
            request.Capacity,
            request.Description
        );
    }

    public static UpdateRoomCommand ToCommand(this UpdateRoomRequest request, Guid id)
    {
        return new UpdateRoomCommand(
            id,
            request.Name,
            request.Capacity,
            request.Description,
            request.IsActive
        );
    }

    public static RoomResponse ToResponse(this RoomDto dto)
    {
        return new RoomResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Capacity = dto.Capacity,
            Description = dto.Description ?? string.Empty,
            IsActive = dto.IsActive
        };
    }
}
