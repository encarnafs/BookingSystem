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
    /// Reglas de autorización:
    /// - Solo los roles <b>Admin</b> y <b>User</b> pueden crear clientes.
    ///
    /// Reglas de negocio:
    /// - El email es obligatorio y debe ser único.
    /// - Se valida que los datos sean correctos.
    /// - Devuelve <b>201 Created</b> con el cliente recién creado.
    /// </remarks>
    /// <response code="201">Cliente creado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="409">Conflicto: el email ya está registrado.</response>
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
    /// Reglas de autorización:
    /// - Admin puede actualizar cualquier cliente.
    /// - Un cliente solo puede actualizar sus propios datos.
    ///
    /// Reglas de negocio:
    /// - El cliente debe existir.
    /// - Los datos deben ser válidos.
    /// - Devuelve <b>204 NoContent</b> si la actualización es correcta.
    /// </remarks>
    /// <response code="204">Cliente actualizado correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para actualizar este cliente.</response>
    /// <response code="404">Cliente no encontrado.</response>
    /// <response code="409">Conflicto: email duplicado.</response>
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
    /// Reglas de autorización:
    /// - Solo Admin y User pueden ver la lista completa.
    ///
    /// Reglas de negocio:
    /// - Devuelve solo clientes activos/no eliminados.
    /// - Si no hay clientes, devuelve una lista vacía.
    /// </remarks>
    /// <response code="200">Listado de clientes devuelto correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
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
    /// Reglas de autorización:
    /// - Admin puede ver cualquier cliente.
    /// - Un cliente solo puede ver sus propios datos.
    ///
    /// Seguridad:
    /// - Si el usuario no tiene permiso, se devuelve <b>403 Forbidden</b>
    ///   incluso si el cliente no existe, para no revelar información.
    ///
    /// Reglas de negocio:
    /// - Devuelve <b>200 OK</b> si el cliente existe.
    /// - Devuelve <b>404 NotFound</b> solo cuando el usuario tiene permiso para ver el recurso.
    /// </remarks>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Cliente no encontrado.</response>
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
            return Forbid(); // Seguridad: no revelar existencia del recurso

        var dto = await _mediator.Send(new GetClientByIdQuery(id));
        return Ok(dto.ToResponse());
    }
}