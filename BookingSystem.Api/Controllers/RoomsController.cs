using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Rooms;
using BookingSystem.Api.Responses.Rooms;
using BookingSystem.Application.Rooms.Commands.CreateRoom;
using BookingSystem.Application.Rooms.Commands.UpdateRoom;
using BookingSystem.Application.Rooms.Queries.CheckAvailability;
using BookingSystem.Application.Rooms.Queries.GetAllRooms;
using BookingSystem.Application.Rooms.Queries.GetRoomById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva sala.
    /// </summary>
    /// <param name="request">Datos necesarios para crear la sala.</param>
    /// <returns>La sala creada.</returns>
    /// <remarks>
    /// Solo los administradores pueden crear salas.
    ///
    /// Reglas de negocio:
    /// - El nombre es obligatorio y debe ser único.
    /// - La descripción es obligatoria.
    /// - La capacidad es obligatoria y debe ser mayor que 0.
    /// - Se devuelve <b>201 Created</b> con la sala recién creada.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RoomResponse>> Create(CreateRoomRequest request)
    {
        var command = request.ToCommand();
        var id = await _mediator.Send(command);

        var room = await _mediator.Send(new GetRoomByIdQuery(id));
        return CreatedAtAction(nameof(GetById), new { id }, room.ToResponse());
    }

    /// <summary>
    /// Actualiza los datos de una sala existente.
    /// </summary>
    /// <param name="id">Identificador de la sala.</param>
    /// <param name="request">Datos actualizados de la sala.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Solo los administradores pueden actualizar salas.
    ///
    /// Reglas de negocio:
    /// - La sala debe existir.
    /// - El nombre es obligatorio y debe ser único.
    /// - La descripción es obligatoria.
    /// - La capacidad es obligatoria y debe ser mayor que 0.
    /// - Si no se envía el campo <c>isActive</c>, se interpreta como <c>false</c> (la sala queda desactivada).
    /// - Devuelve <b>204 NoContent</b> si la actualización se realiza correctamente.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateRoomRequest request)
    {
        var command = request.ToCommand(id);
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Obtiene una sala por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la sala.</param>
    /// <returns>Los datos de la sala solicitada.</returns>
    /// <remarks>
    /// Disponible para administradores y usuarios autenticados.
    ///
    /// Reglas de negocio:
    /// - La sala debe existir.
    /// - Requiere autenticación.
    /// - Si no existe, se devuelve <b>404 NotFound</b>.
    /// </remarks>
    [Authorize(Roles = "Admin,User")]
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponse>> GetById(Guid id)
    {
        var room = await _mediator.Send(new GetRoomByIdQuery(id));
        return Ok(room.ToResponse());
    }

    /// <summary>
    /// Obtiene todas las salas del sistema.
    /// </summary>
    /// <returns>Una colección con todas las salas.</returns>
    /// <remarks>
    /// Disponible para administradores, usuarios y clientes autenticados.
    /// 
    /// Reglas de negocio:
    /// - Devuelve todas las salas activas del sistema.
    /// - Si no hay salas registradas, devuelve una lista vacía.
    /// - Requiere autenticación.
    /// </remarks>
    [Authorize(Roles = "Admin,User,Client")]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<RoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetAll()
    {
        var rooms = await _mediator.Send(new GetAllRoomsQuery());
        var response = rooms.Select(r => r.ToResponse()).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Comprueba la disponibilidad de una sala en un rango de fechas.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite verificar si una sala está disponible entre dos fechas.
    ///
    /// - Devuelve <b>isAvailable = true</b> si no existe ninguna reserva que solape.
    /// - Devuelve <b>isAvailable = false</b> junto con la lista de reservas que causan conflicto.
    /// - Siempre devuelve <b>200 OK</b>, incluso cuando la sala no está disponible.
    ///
    /// Reglas de negocio:
    /// - La sala debe existir.
    /// - Las fechas deben ser válidas (la fecha de inicio debe ser anterior a la fecha de fin).
    /// - Requiere autenticación.
    /// - Se devuelven todas las reservas que solapan, incluyendo su estado actual.
    ///
    /// <b>Comportamiento según el rol del usuario:</b>
    /// - Client → respuesta reducida.
    /// - Admin/User → respuesta completa.
    /// </remarks>
    /// <param name="roomId">Identificador de la sala.</param>
    /// <param name="start">Fecha de inicio del rango.</param>
    /// <param name="end">Fecha de fin del rango.</param>
    /// <returns>Información de disponibilidad y reservas en conflicto.</returns>
    /// <response code="200">Consulta realizada correctamente.</response>
    /// <response code="400">Datos inválidos (fechas incorrectas, formato inválido, etc.).</response>
    /// <response code="401">No autorizado.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpGet("{roomId:guid}/availability")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> CheckAvailability(
        Guid roomId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        var dto = await _mediator.Send(new CheckRoomAvailabilityQuery(roomId, start, end));

        // Si es Client → respuesta reducida
        if (User.IsInRole("Client"))
        {
            return Ok(new RoomAvailabilityClientResponse
            {
                IsAvailable = dto.IsAvailable
            });
        }

        // Si es Admin o User → respuesta completa
        return Ok(dto.ToResponse());
    }

}
