using MediatR;

namespace BookingSystem.Application.Rooms.Commands.CreateRoom;

public record CreateRoomCommand(
    string Name,
    int Capacity,
    string? Description
) : IRequest<Guid>;
