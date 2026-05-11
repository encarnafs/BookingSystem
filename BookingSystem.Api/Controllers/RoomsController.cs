using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Rooms;
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
    /// Crea una nueva habitación.
    /// </summary>
    /// <param name="request">Datos necesarios para crear la habitación.</param>
    /// <returns>El identificador de la habitación creada.</returns>
    /// <remarks>
    /// Solo los administradores pueden crear habitaciones.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomRequest request)
    {
        var command = request.ToCommand();
        var id = await _mediator.Send(command);

        return Ok(id);
    }

    /// <summary>
    /// Actualiza los datos de una habitación existente.
    /// </summary>
    /// <param name="id">Identificador de la habitación.</param>
    /// <param name="request">Datos actualizados de la habitación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Solo los administradores pueden actualizar habitaciones.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoomRequest request)
    {
        var command = request.ToCommand(id);
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Obtiene una habitación por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la habitación.</param>
    /// <returns>Los datos de la habitación solicitada.</returns>
    /// <remarks>
    /// Disponible para administradores y usuarios autenticados.
    /// </remarks>
    [Authorize(Roles = "Admin,User")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var room = await _mediator.Send(new GetRoomByIdQuery(id));
        return Ok(room.ToResponse());
    }

    /// <summary>
    /// Obtiene todas las habitaciones del sistema.
    /// </summary>
    /// <returns>Una colección con todas las habitaciones.</returns>
    /// <remarks>
    /// Disponible para administradores y usuarios autenticados.
    /// </remarks>
    [Authorize(Roles = "Admin,User")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _mediator.Send(new GetAllRoomsQuery());

        var response = rooms
            .Select(r => r.ToResponse())
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Comprueba la disponibilidad de una habitación en un rango de fechas.
    /// </summary>
    /// <param name="roomId">Identificador de la habitación.</param>
    /// <param name="start">Fecha de inicio.</param>
    /// <param name="end">Fecha de fin.</param>
    /// <returns>La disponibilidad de la habitación en el rango solicitado.</returns>
    /// <remarks>
    /// Este endpoint no requiere rol específico; cualquier usuario autenticado puede consultar disponibilidad.
    /// </remarks>
    [HttpGet("{roomId:guid}/availability")]
    public async Task<IActionResult> CheckAvailability(
        Guid roomId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        var dto = await _mediator.Send(new CheckRoomAvailabilityQuery(roomId, start, end));
        return Ok(dto.ToResponse());
    }
}


