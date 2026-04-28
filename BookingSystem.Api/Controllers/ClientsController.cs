using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Clients;
using BookingSystem.Application.Clients.Commands.CreateClient;
using BookingSystem.Application.Clients.Commands.UpdateClient;
using BookingSystem.Application.Clients.Queries.GetAllClients;
using BookingSystem.Application.Clients.Queries.GetClientById;
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
    public async Task<IActionResult> Create(CreateClientRequest request)
    {
        var id = await _mediator.Send(request.ToCommand());
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClientRequest request)
    {
        await _mediator.Send(request.ToCommand(id));
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dtos = await _mediator.Send(new GetAllClientsQuery());
        return Ok(dtos.Select(d => d.ToResponse()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetClientByIdQuery(id));
        return Ok(dto.ToResponse());
    }
}
