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
using BookingSystem.Application.Clients.Queries.GetClientById;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Rooms.Queries.GetRoomById;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
    /// <remarks>
    /// Reglas de autorización:
    /// - Los roles <b>Admin</b> y <b>User</b> pueden acceder a cualquier reserva.
    /// - El rol <b>Client</b> solo puede acceder a sus propias reservas.
    ///
    /// Reglas de negocio:
    /// - La reserva debe existir.
    /// </remarks>
    /// <param name="id">Identificador único de la reserva (GUID).</param>
    /// <returns>Los datos de la reserva solicitada.</returns>
    /// <response code="200">Reserva encontrada y devuelta correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para acceder a esta reserva.</response>
    /// <response code="404">La reserva no existe.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id));

        if (booking is null)
            return NotFound();

        if (!User.IsInRole("Admin") && !User.IsInRole("User") && booking.ClientId != currentUserId)
            return Forbid();

        return Ok(booking.ToResponse());
    }

    /// <summary>
    /// Obtiene todas las reservas asociadas a una habitación.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - La habitación debe existir.
    /// - Solo los usuarios con rol <b>Admin</b> o <b>User</b> pueden consultar las reservas de una habitación.
    /// - Los usuarios con rol <b>Client</b> no tienen acceso a este recurso.
    /// - Se devuelven todas las reservas asociadas a la habitación, independientemente del cliente.
    /// </remarks>
    /// <param name="roomId">Identificador único de la habitación (GUID).</param>
    /// <returns>Una colección de reservas asociadas a la habitación.</returns>
    /// <response code="200">Listado de reservas devuelto correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para consultar estas reservas.</response>
    /// <response code="404">La habitación no existe.</response>
    [Authorize(Roles = "Admin, User")]
    [HttpGet("room/{roomId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByRoom(Guid roomId)
    {
        // Validación de existencia de la habitación
        var room = await _sender.Send(new GetRoomByIdQuery(roomId));
        if (room is null)
            return NotFound();

        var result = await _sender.Send(new GetBookingsByRoomIdQuery(roomId));
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Obtiene todas las reservas asociadas a un cliente.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - El cliente debe existir.
    /// - Un usuario con rol <b>Client</b> solo puede ver sus propias reservas.
    /// - Los usuarios con rol <b>Admin</b> o <b>User</b> pueden consultar las reservas de cualquier cliente.
    /// - No se permite acceder a reservas de otros clientes sin autorización.
    /// </remarks>
    /// <param name="clientId">Identificador único del cliente (GUID).</param>
    /// <returns>Una colección de reservas asociadas al cliente.</returns>
    /// <response code="200">Listado de reservas devuelto correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para consultar estas reservas.</response>
    /// <response code="404">El cliente no existe.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpGet("client/{clientId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByClient(Guid clientId)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        // Validación de existencia del cliente
        var client = await _sender.Send(new GetClientByIdQuery(clientId));
        if (client is null)
            return NotFound();

        // Autorización: Admin y User pueden ver cualquier cliente; Client solo el suyo
        if (!User.IsInRole("Admin") && !User.IsInRole("User") && clientId != currentUserId)
            return Forbid();

        var result = await _sender.Send(new GetBookingsByClientIdQuery(clientId));
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Obtiene todas las reservas del sistema.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - Solo los usuarios con rol <b>Admin</b> o <b>User</b> pueden acceder al listado completo de reservas.
    /// - Los usuarios con rol <b>Client</b> no tienen acceso a este recurso.
    /// - Se devuelven todas las reservas del sistema, independientemente del cliente o la habitación.
    /// </remarks>
    /// <returns>Una colección con todas las reservas.</returns>
    /// <response code="200">Listado de reservas devuelto correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para consultar todas las reservas.</response>
    [Authorize(Roles = "Admin, User")]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAll()
    {
        // Validación manual de autenticación
        if (_currentUser.UserId is null)
            return Unauthorized();

        // Validación manual de autorización
        if (!User.IsInRole("Admin") && !User.IsInRole("User"))
            return Forbid();

        var result = await _sender.Send(new GetAllBookingsQuery());
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Obtiene todas las reservas dentro de un rango de fechas.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - Solo los usuarios con rol <b>Admin</b> o <b>User</b> pueden consultar reservas por rango de fechas.
    /// - Los usuarios con rol <b>Client</b> no tienen acceso a este recurso.
    /// - Las fechas deben tener un formato válido.
    /// - La fecha de inicio debe ser anterior a la fecha de fin.
    /// - Se devuelven todas las reservas que se encuentren total o parcialmente dentro del rango especificado.
    /// </remarks>
    /// <param name="start">Fecha de inicio del rango.</param>
    /// <param name="end">Fecha de fin del rango.</param>
    /// <returns>Una colección de reservas dentro del rango especificado.</returns>
    /// <response code="200">Listado de reservas devuelto correctamente.</response>
    /// <response code="400">Fechas inválidas o rango incorrecto.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para consultar estas reservas.</response>
    [Authorize(Roles = "Admin, User")]
    [HttpGet("daterange")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetInDateRange(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        // Validación manual de autenticación
        if (_currentUser.UserId is null)
            return Unauthorized();

        // Validación manual de autorización
        if (!User.IsInRole("Admin") && !User.IsInRole("User"))
            return Forbid();

        // Validación del rango de fechas
        if (start >= end)
            return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");

        var result = await _sender.Send(new GetBookingsInDateRangeQuery(start, end));
        return Ok(result.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Crea una nueva reserva.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - La sala debe existir.
    /// - El cliente debe existir (excepto cuando el usuario autenticado es <b>Client</b>).
    /// - Los usuarios con rol <b>Client</b> solo pueden crear reservas para sí mismos.
    /// - Los usuarios con rol <b>Admin</b> o <b>User</b> pueden crear reservas para cualquier cliente.
    /// - Si el usuario autenticado es <b>Client</b>, el campo <b>clientId</b> debe omitirse.
    /// - El campo <b>comments</b> es opcional; si no se envía, se establece como <c>null</c>.
    /// - El estado inicial de la reserva será <b>Pending</b>.
    /// - Las fechas no pueden solaparse con reservas existentes.
    /// - La fecha de inicio debe ser anterior a la fecha de fin.
    /// </remarks>
    /// <param name="request">Datos necesarios para crear la reserva.</param>
    /// <returns>La reserva creada.</returns>
    /// <response code="201">Reserva creada correctamente.</response>
    /// <response code="400">Datos inválidos (fechas incorrectas, formato inválido, etc.).</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para crear esta reserva.</response>
    /// <response code="404">La sala o el cliente no existen.</response>
    /// <response code="409">Conflicto: la sala ya está reservada en ese rango de fechas.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpPost]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponse>> Create(CreateBookingRequest request)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var createdByUserId = _currentUser.UserId.Value;

        // Validación mínima: un Client no debe enviar clientId
        if (User.IsInRole("Client") && request.ClientId is not null)
            return BadRequest("Los usuarios con rol Client no deben enviar el campo clientId.");

        // Validación de autorización: Client solo puede crear para sí mismo
        if (!User.IsInRole("Admin") && !User.IsInRole("User") &&
            request.ClientId is not null && request.ClientId != createdByUserId)
            return Forbid();

        var clientId = request.ClientId ?? createdByUserId;

        var command = request.ToCommand(createdByUserId);

        var result = await _sender.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToResponse());
    }

    /// <summary>
    /// Actualiza las fechas de una reserva existente.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - La reserva debe existir.
    /// - Los usuarios con rol <b>Admin</b>, <b>User</b> o el <b>Client</b> dueño de la reserva pueden modificar las fechas.
    /// - No se permite modificar las fechas de reservas pertenecientes a otros clientes sin autorización.
    /// - La fecha de inicio debe ser anterior a la fecha de fin.
    /// - Las nuevas fechas no pueden solaparse con otras reservas existentes de la misma sala.
    /// - Solo se actualizan las fechas; el resto de campos de la reserva permanecen sin cambios.
    /// </remarks>
    /// <param name="id">Identificador único de la reserva.</param>
    /// <param name="request">Nuevas fechas de inicio y fin.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <response code="204">Fechas actualizadas correctamente.</response>
    /// <response code="400">Datos inválidos (fechas incorrectas, formato inválido, etc.).</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para modificar esta reserva.</response>
    /// <response code="404">La reserva no existe.</response>
    /// <response code="409">Conflicto: las nuevas fechas se solapan con otra reserva existente.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpPatch("{id:guid}/dates")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateDates(
        Guid id,
        UpdateBookingDatesRequest request,
        CancellationToken cancellationToken)
    {
        // Autenticación
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        // Validación mínima del rango de fechas
        if (request.Start >= request.End)
            return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");

        // Obtener reserva
        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (booking is null)
            return NotFound();

        // Autorización
        if (!User.IsInRole("Admin") && !User.IsInRole("User") && booking.ClientId != currentUserId)
            return Forbid();

        // Ejecutar comando
        var command = new UpdateBookingDatesCommand(id, request.Start, request.End);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Actualiza los comentarios de una reserva existente.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - La reserva debe existir.
    /// - Los usuarios con rol <b>Admin</b>, <b>User</b> o el <b>Client</b> dueño de la reserva pueden modificar los comentarios.
    /// - No se permite modificar reservas de otros clientes sin autorización.
    /// - El campo <b>comments</b> puede ser <c>null</c> o una cadena vacía.
    /// - El dominio valida el formato y las reglas adicionales.
    /// </remarks>
    /// <param name="id">Identificador único de la reserva.</param>
    /// <param name="request">Nuevos comentarios.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <response code="204">Comentarios actualizados correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para modificar esta reserva.</response>
    /// <response code="404">La reserva no existe.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpPatch("{id:guid}/comments")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComments(
        Guid id,
        UpdateBookingCommentsRequest request,
        CancellationToken cancellationToken)
    {
        // Autenticación
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        // Validación mínima del campo comments
        if (request.Comments is not null && string.IsNullOrWhiteSpace(request.Comments))
            return BadRequest("El campo comments no puede contener solo espacios en blanco.");

        // Obtener reserva
        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (booking is null)
            return NotFound();

        // Autorización
        if (!User.IsInRole("Admin") && !User.IsInRole("User") && booking.ClientId != currentUserId)
            return Forbid();

        // Ejecutar comando
        var command = new UpdateBookingCommentsCommand(id, request.Comments);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Confirma una reserva existente.
    /// </summary>
    /// <remarks>
    /// Reglas de autorización:
    /// - Los roles <b>Admin</b> y <b>User</b> pueden confirmar cualquier reserva.
    /// - El rol <b>Client</b> solo puede confirmar sus propias reservas.
    ///
    /// Reglas de negocio:
    /// - La reserva debe existir.
    /// - Las validaciones de negocio (estado no válido, ya confirmada, etc.)
    ///   se gestionan en el dominio y se devuelven como <c>ProblemDetails</c>
    ///   mediante el middleware de excepciones.
    /// </remarks>
    /// <param name="id">Identificador único de la reserva.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <response code="204">Reserva confirmada correctamente.</response>
    /// <response code="400">Datos inválidos o estado no válido.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para confirmar esta reserva.</response>
    /// <response code="404">La reserva no existe.</response>
    /// <response code="409">Conflicto: la reserva no puede confirmarse en su estado actual.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpPost("{id:guid}/confirm")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);
        if (booking is null)
            return NotFound();

        // Autorización unificada (igual que en UpdateDates y UpdateComments)
        if (!User.IsInRole("Admin") &&
            !User.IsInRole("User") &&
            booking.ClientId != currentUserId)
        {
            return Forbid();
        }

        await _sender.Send(new ConfirmBookingCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Cancela una reserva existente.
    /// </summary>
    /// <remarks>
    /// Reglas de negocio:
    ///
    /// - La reserva debe existir.
    /// - Los usuarios con rol <b>Admin</b>, <b>User</b> o el <b>Client</b> dueño de la reserva pueden cancelarla.
    /// - No se permite cancelar reservas de otros clientes sin autorización.
    /// - El dominio valida el estado actual de la reserva.
    /// </remarks>
    /// <param name="id">Identificador único de la reserva.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <response code="204">Reserva cancelada correctamente.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para cancelar esta reserva.</response>
    /// <response code="404">La reserva no existe.</response>
    /// <response code="409">Conflicto: la reserva no puede cancelarse debido a su estado actual.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpPatch("{id:guid}/cancel")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (booking is null)
            return NotFound();

        if (!User.IsInRole("Admin") && !User.IsInRole("User") && booking.ClientId != currentUserId)
            return Forbid();

        await _sender.Send(new CancelBookingCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Actualiza una reserva existente.
    /// </summary>
    /// <remarks>
    /// Reglas de autorización:
    /// - Los roles <b>Admin</b> y <b>User</b> pueden actualizar cualquier reserva.
    /// - El rol <b>Client</b> solo puede actualizar sus propias reservas.
    ///
    /// Reglas de negocio:
    /// - La reserva debe existir.
    /// - Las validaciones de negocio (solapamientos, estados no válidos, etc.)
    ///   se gestionan en el dominio y se devuelven como <c>ProblemDetails</c>
    ///   mediante el middleware de excepciones.
    /// </remarks>
    /// <param name="id">Identificador único de la reserva a actualizar.</param>
    /// <param name="request">Datos actualizados de la reserva.</param>
    /// <returns>Sin contenido si la operación es exitosa.</returns>
    /// <response code="204">Reserva actualizada correctamente.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autorizado.</response>
    /// <response code="403">Prohibido: el usuario no tiene permisos para modificar esta reserva.</response>
    /// <response code="404">La reserva no existe.</response>
    /// <response code="409">Conflicto: las fechas se solapan con otra reserva existente.</response>
    [Authorize(Roles = "Admin, User, Client")]
    [HttpPut("{id:guid}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var currentUserId = _currentUser.UserId.Value;

        var booking = await _sender.Send(new GetBookingByIdQuery(id), cancellationToken);

        if (booking is null)
            return NotFound();

        if (!User.IsInRole("Admin") && !User.IsInRole("User") && booking.ClientId != currentUserId)
            return Forbid();

        var command = request.ToCommand(id, booking.ClientId);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}

