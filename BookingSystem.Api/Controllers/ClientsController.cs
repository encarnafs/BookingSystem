using BookingSystem.Application.Clients.Commands.CreateClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClientCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
