using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Rooms;
using BookingSystem.Application.Rooms.Queries.GetRoomById;
using BookingSystem.Application.Rooms.Queries.GetAllRooms;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomRequest request)
    {
        var command = request.ToCommand();
        var id = await _mediator.Send(command);

        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoomRequest request)
    {
        var command = request.ToCommand(id);
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var room = await _mediator.Send(new GetRoomByIdQuery(id));
        return Ok(room.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _mediator.Send(new GetAllRoomsQuery());

        var response = rooms
            .Select(r => r.ToResponse())
            .ToList();

        return Ok(response);
    }
}

