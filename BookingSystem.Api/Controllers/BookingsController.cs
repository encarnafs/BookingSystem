
using BookingSystem.Api.Mappers;
using BookingSystem.Api.Requests.Bookings;
using BookingSystem.Application.Bookings.Commands.CancelBooking;
using BookingSystem.Application.Bookings.Commands.ConfirmBooking;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBookingComments;
using BookingSystem.Application.Bookings.Commands.UpdateBookingDates;
using BookingSystem.Application.Bookings.Queries.GetBookingById;
using BookingSystem.Application.Bookings.Queries.GetBookingsByClientId;
using BookingSystem.Application.Bookings.Queries.GetBookingsByRoomId;
using BookingSystem.Application.Bookings.Queries.GetAllBookings;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetBookingByIdQuery(id));
        return Ok(result.ToResponse());
    }

    [HttpGet("room/{roomId:guid}")]
    public async Task<IActionResult> GetByRoom(Guid roomId)
    {
        var result = await _sender.Send(new GetBookingsByRoomIdQuery(roomId));
        return Ok(result.Select(b => b.ToResponse()));
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
    {
        var result = await _sender.Send(new GetBookingsByClientIdQuery(clientId));
        return Ok(result.Select(b => b.ToResponse()));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetAllBookingsQuery());
        return Ok(result.Select(b => b.ToResponse()));
    }

    [HttpGet("daterange")]
    public async Task<IActionResult> GetInDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var result = await _sender.Send(new GetBookingsInDateRangeQuery(start, end));
        return Ok(result.Select(b => b.ToResponse()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var command = request.ToCommand(Guid.Parse(_currentUser.UserId));

        var result = await _sender.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = result }, result.ToResponse());

    }


    [HttpPut("{id:guid}/dates")]
    public async Task<IActionResult> UpdateDates(Guid id, UpdateBookingDatesRequest request)
    {
        var command = new UpdateBookingDatesCommand(
            id,
            request.Start,
            request.End
        );

        await _sender.Send(command);
        return NoContent();
    }

    [HttpPatch("{id:guid}/comments")]
    public async Task<IActionResult> UpdateComments(Guid id, UpdateBookingCommentsRequest request)
    {
        var command = request.ToCommand(id);

        await _sender.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _sender.Send(new ConfirmBookingCommand(id));
        return NoContent();
    }

    // ¿Por qué no usamos [Authorize(Policy = "...")] aquí?
    // Porque esa sintaxis no permite pasar el bookingId al handler.
    // Y mi policy necesita el ID de la reserva para consultar el repositorio.
        [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        // Ejecutar la policy con el bookingId como resource
        var authorizationResult = await _authorizationService.AuthorizeAsync(
            User,
            id, // resource
            "CanCancelBooking"
        );

        if (!authorizationResult.Succeeded)
            return Forbid();

        await _sender.Send(new CancelBookingCommand(id));
        return NoContent();
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}
