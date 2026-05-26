using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Clients;
using BookingSystem.Api.Responses.Clients;
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
    /// <returns>El cliente creado.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - Solo los administradores y usuarios internos pueden crear clientes.
    /// - El email debe ser único.
    /// - Devuelve <b>201 Created</b> con el cliente recién creado.
    /// 
    /// Seguridad:
    /// - Requiere autenticación.
    /// - Roles permitidos: Admin, User.
    /// </remarks>
    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ClientResponse>> Create(CreateClientRequest request)
    {
        var clientDto = await _mediator.Send(request.ToCommand());
        return CreatedAtAction(nameof(GetById), new { id = clientDto.Id }, clientDto.ToResponse());
    }

    /// <summary>
    /// Actualiza los datos de un cliente existente.
    /// </summary>
    /// <param name="id">Identificador del cliente.</param>
    /// <param name="request">Datos actualizados del cliente.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Reglas de negocio:
    /// - El cliente debe existir.
    /// - Un cliente solo puede actualizar sus propios datos.
    /// - El administrador puede actualizar cualquier cliente.
    /// - Devuelve <b>204 NoContent</b> si la actualización es correcta.
    /// 
    /// Seguridad:
    /// - Requiere autenticación.
    /// - Si el usuario no es Admin y no coincide su ID → 403 Forbidden.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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
    /// Reglas de negocio:
    /// - Solo administradores y usuarios internos pueden ver la lista completa.
    /// - Devuelve <b>200 OK</b> con la colección de clientes.
    /// 
    /// Seguridad:
    /// - Requiere autenticación.
    /// - Roles permitidos: Admin, User.
    /// </remarks>
    [Authorize(Roles = "Admin, User")]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<ClientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll()
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
    /// Reglas de negocio:
    /// - Un cliente solo puede ver sus propios datos.
    /// - El administrador puede ver cualquier cliente.
    /// - Devuelve <b>200 OK</b> si el cliente existe.
    /// - Devuelve <b>404 NotFound</b> si no existe.
    /// 
    /// Seguridad:
    /// - Requiere autenticación.
    /// - Si el usuario no es Admin y no coincide su ID → 403 Forbidden.
    /// </remarks>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ClientResponse>> GetById(Guid id)
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