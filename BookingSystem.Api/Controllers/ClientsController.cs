using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Clients;
using BookingSystem.Application.Clients.Commands.CreateClient;
using BookingSystem.Application.Clients.Commands.UpdateClient;
using BookingSystem.Application.Clients.Queries.GetAllClients;
using BookingSystem.Application.Clients.Queries.GetClientById;
using BookingSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ClientsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Crea un nuevo cliente.
    /// </summary>
    /// <param name="request">Datos necesarios para crear el cliente.</param>
    /// <returns>El identificador del cliente creado.</returns>
    /// <remarks>
    /// Solo los administradores pueden crear clientes.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateClientRequest request)
    {
        var id = await _mediator.Send(request.ToCommand());
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="request">Datos actualizados del cliente.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Un cliente solo puede actualizar sus propios datos.  
    /// El administrador puede actualizar cualquier cliente.
    /// </remarks>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClientRequest request)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        if (!User.IsInRole("Admin") && currentUserId != id)
            return Forbid();

        await _mediator.Send(request.ToCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Obtiene todos los clientes del sistema.
    /// </summary>
    /// <returns>Una colección con todos los clientes.</returns>
    /// <remarks>
    /// Solo los administradores pueden ver la lista completa de clientes.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dtos = await _mediator.Send(new GetAllClientsQuery());
        return Ok(dtos.Select(d => d.ToResponse()));
    }

    /// <summary>
    /// Obtiene un cliente por su identificador único.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <returns>Los datos del cliente solicitado.</returns>
    /// <remarks>
    /// Un cliente solo puede ver sus propios datos.  
    /// El administrador puede ver cualquier cliente.
    /// </remarks>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        if (!User.IsInRole("Admin") && currentUserId != id)
            return Forbid();

        var dto = await _mediator.Send(new GetClientByIdQuery(id));
        return Ok(dto.ToResponse());
    }
}
