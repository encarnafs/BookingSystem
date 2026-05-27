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
    /// <b>Reglas de autorización:</b>
    /// - Solo los administradores pueden crear salas.
    ///
    /// <b>Reglas de negocio:</b>
    /// - El nombre es obligatorio y debe ser único.
    /// - La descripción es obligatoria.
    /// - La capacidad es obligatoria y debe ser mayor que 0.
    ///
    /// <b>Respuestas:</b>
    /// <response code="201">Sala creada correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="409">Conflicto: nombre duplicado.</response>
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("application/json")]
    //// CAMBIO: Añadido para homogeneizar con BookingsController
    [Produces("application/json")]
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
    /// <b>Reglas de autorización:</b>
    /// - Solo los administradores pueden actualizar salas.
    ///
    /// <b>Reglas de negocio:</b>
    /// - La sala debe existir.
    /// - El nombre es obligatorio y debe ser único.
    /// - La descripción es obligatoria.
    /// - La capacidad debe ser mayor que 0.
    /// - Si no se envía <c>isActive</c>, se interpreta como <c>false</c>.
    ///
    /// <b>Respuestas:</b>
    /// <response code="204">Actualizada correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Sala no encontrada.</response>
    /// <response code="409">Conflicto: nombre duplicado.</response>
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    //// CAMBIO: Añadido para homogeneizar con BookingsController
    [Produces("application/json")]
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
    /// <b>Reglas de autorización:</b>
    /// - Roles permitidos: Admin, User.
    ///
    /// <b>Reglas de negocio:</b>
    /// - La sala debe existir.
    ///
    /// <b>Respuestas:</b>
    /// <response code="200">Sala encontrada.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
    /// <response code="404">Sala no encontrada.</response>
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
    /// <b>Reglas de autorización:</b>
    /// - Roles permitidos: Admin, User, Client.
    ///
    /// <b>Reglas de negocio:</b>
    /// - Devuelve solo salas activas.
    /// - Si no hay salas, devuelve una lista vacía.
    ///
    /// <b>Respuestas:</b>
    /// <response code="200">Listado devuelto correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido.</response>
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
    /// <b>Reglas de autorización:</b>
    /// - Roles permitidos: Admin, User, Client.
    ///
    /// <b>Reglas de negocio:</b>
    /// - La sala debe existir.
    /// - La fecha de inicio debe ser anterior a la fecha de fin.
    /// - Devuelve isAvailable y, si aplica, las reservas que solapan.
    ///
    /// <b>Comportamiento según rol:</b>
    /// - Client → respuesta reducida.
    /// - Admin/User → respuesta completa.
    ///
    /// <b>Respuestas:</b>
    /// <response code="200">Consulta realizada correctamente.</response>
    /// <response code="400">Fechas inválidas.</response>
    /// <response code="401">No autorizado.</response>
    /// </remarks>
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
        //// CAMBIO: Validación mínima igual que en BookingsController
        if (start == default || end == default)
            return BadRequest("Las fechas no pueden estar vacías.");

        if (start >= end)
            return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");

        var dto = await _mediator.Send(new CheckRoomAvailabilityQuery(roomId, start, end));

        if (User.IsInRole("Client"))
        {
            return Ok(new RoomAvailabilityClientResponse
            {
                IsAvailable = dto.IsAvailable
            });
        }

        return Ok(dto.ToResponse());
    }
}