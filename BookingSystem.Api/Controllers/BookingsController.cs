using BookingSystem.Api.Requests.Bookings;
using BookingSystem.Application.Bookings.Commands.CancelBooking;
using BookingSystem.Application.Bookings.Commands.ConfirmBooking;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBookingComments;
using BookingSystem.Application.Bookings.Commands.UpdateBookingDates;
using BookingSystem.Application.Bookings.Queries.GetBookingById;
using BookingSystem.Application.Bookings.Queries.GetBookingsByClientId;
using BookingSystem.Application.Bookings.Queries.GetBookingsByRoomId;
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

    public BookingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetBookingByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("room/{roomId:guid}")]
    public async Task<IActionResult> GetByRoom(Guid roomId)
    {
        var result = await _sender.Send(new GetBookingsByRoomIdQuery(roomId));
        return Ok(result);
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
    {
        var result = await _sender.Send(new GetBookingsByClientIdQuery(clientId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        var command = new CreateBookingCommand(
            request.RoomId,
            request.ClientId,
            Guid.Empty,
            request.Start,
            request.End,
            request.Comments
        );

        var result = await _sender.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = result }, result);
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
        var command = new UpdateBookingCommentsCommand(
            id,
            request.Comments
        );

        await _sender.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _sender.Send(new ConfirmBookingCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _sender.Send(new CancelBookingCommand(id));
        return NoContent();
    }
}
