using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Bookings;
using BookingSystem.Api.Responses.Bookings;
using BookingSystem.Application.Bookings.Commands.CancelBooking;
using BookingSystem.Application.Bookings.Commands.ConfirmBooking;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBookingComments;
using BookingSystem.Application.Bookings.Commands.UpdateBookingDates;
using BookingSystem.Application.Bookings.Queries.GetAllBookings;
using BookingSystem.Application.Bookings.Queries.GetBookingById;
using BookingSystem.Application.Bookings.Queries.GetBookingsByClientId;
using BookingSystem.Application.Bookings.Queries.GetBookingsByRoomId;
using BookingSystem.Application.Bookings.Queries.GetBookingsInDateRange;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUser;

    public BookingsController(ISender sender, IAuthorizationService authorizationService, ICurrentUserService currentUser)
    {
        _sender = sender;
        _authorizationService = authorizationService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtiene una reserva por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la reserva (GUID).</param>
    /// <returns>Los datos de la reserva solicitada.</returns>
    /// <remarks>
    /// Solo el administrador o el cliente dueño de la reserva pueden acceder a ella.
    /// </remarks>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id));

        if (!User.IsInRole("Admin") && booking.ClientId != currentUserId)
            return Forbid();

        return Ok(booking.ToResponse());
    }

    /// <summary>
    /// Obtiene todas las reservas asociadas a una habitación.
    /// </summary>
    /// <param name="roomId">Identificador de la habitación.</param>
    /// <returns>Una colección de reservas asociadas a la habitación.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("room/{roomId:guid}")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByRoom(Guid roomId)
    {
        var result = await _sender.Send(new GetBookingsByRoomIdQuery(roomId));
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Obtiene todas las reservas asociadas a un cliente.
    /// </summary>
    /// <param name="clientId">Identificador del cliente.</param>
    /// <returns>Una colección de reservas del cliente.</returns>
    /// <remarks>
    /// Un cliente solo puede ver sus propias reservas. El administrador puede ver las de cualquier cliente.
    /// </remarks>
    [HttpGet("client/{clientId:guid}")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByClient(Guid clientId)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        if (!User.IsInRole("Admin") && clientId != currentUserId)
            return Forbid();

        var result = await _sender.Send(new GetBookingsByClientIdQuery(clientId));
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Obtiene todas las reservas del sistema.
    /// </summary>
    /// <returns>Una colección con todas las reservas.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAll()
    {
        var result = await _sender.Send(new GetAllBookingsQuery());
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Obtiene todas las reservas dentro de un rango de fechas.
    /// </summary>
    /// <param name="start">Fecha de inicio.</param>
    /// <param name="end">Fecha de fin.</param>
    /// <returns>Una colección de reservas dentro del rango especificado.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("daterange")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetInDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var result = await _sender.Send(new GetBookingsInDateRangeQuery(start, end));
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Crea una nueva reserva.
    /// </summary>
    /// <param name="request">Datos necesarios para crear la reserva.</param>
    /// <returns>La reserva creada.</returns>
    /// <remarks>
    /// Un cliente solo puede crear reservas para sí mismo. El administrador puede crear reservas para cualquier cliente.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create(CreateBookingRequest request)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var createdByUserId = _currentUser.UserId.Value;

        if (!User.IsInRole("Admin") && request.ClientId is not null && request.ClientId != createdByUserId)
            return Forbid();

        var clientId = request.ClientId ?? createdByUserId;

        var command = request.ToCommand(createdByUserId);

        var result = await _sender.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToResponse());
    }

    /// <summary>
    /// Actualiza las fechas de una reserva existente.
    /// </summary>
    /// <param name="id">Identificador de la reserva.</param>
    /// <param name="request">Nuevas fechas de inicio y fin.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    [HttpPatch("{id:guid}/dates")]
    public async Task<IActionResult> UpdateDates(Guid id, UpdateBookingDatesRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (!User.IsInRole("Admin") && booking.ClientId != currentUserId)
            return Forbid();

        var command = new UpdateBookingDatesCommand(id, request.Start, request.End);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Actualiza los comentarios de una reserva.
    /// </summary>
    /// <param name="id">Identificador de la reserva.</param>
    /// <param name="request">Nuevos comentarios.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    [HttpPatch("{id:guid}/comments")]
    public async Task<IActionResult> UpdateComments(Guid id, UpdateBookingCommentsRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (!User.IsInRole("Admin") && booking.ClientId != currentUserId)
            return Forbid();

        var command = new UpdateBookingCommentsCommand(id, request.Comments);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Confirma una reserva.
    /// </summary>
    /// <param name="id">Identificador de la reserva.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Solo el administrador o el cliente dueño de la reserva pueden confirmarla.
    /// </remarks>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (!User.IsInRole("Admin") && booking.ClientId != currentUserId)
            return Forbid();

        await _sender.Send(new ConfirmBookingCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Cancela una reserva.
    /// </summary>
    /// <param name="id">Identificador de la reserva.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// La autorización se realiza mediante la policy "CanCancelBooking".
    /// </remarks>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            id,
            "CanCancelBooking"
        );

        if (!authorizationResult.Succeeded)
            return Forbid();

        await _sender.Send(new CancelBookingCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Actualiza una reserva completa.
    /// </summary>
    /// <param name="id">Identificador de la reserva.</param>
    /// <param name="request">Datos actualizados de la reserva.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <remarks>
    /// Solo el administrador o el cliente dueño de la reserva pueden modificarla.
    /// </remarks>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (!User.IsInRole("Admin") && booking.ClientId != currentUserId)
            return Forbid();

        var command = request.ToCommand(booking.ClientId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}

