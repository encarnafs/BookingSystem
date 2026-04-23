using BookingSystem.Application.Rooms.Commands.CreateRoom;
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
    public async Task<IActionResult> Create(CreateRoomCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
