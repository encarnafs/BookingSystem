using MediatR;

namespace BookingSystem.Application.Rooms.Commands.UpdateRoom;

public record UpdateRoomCommand(
    Guid Id,
    string Name,
    int Capacity,
    string Description,
    bool IsActive
) : IRequest;
