using BookingSystem.Api.Requests.Bookings;
using BookingSystem.Api.Responses.Bookings;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBooking;
using BookingSystem.Application.Bookings.Commands.UpdateBookingComments;
using BookingSystem.Application.Bookings.Commands.UpdateBookingDates;
using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;

namespace BookingSystem.Api.Mappers;

public static class BookingMapper
{
    // -----------------------------
    // REQUEST → COMMAND (Entrada)
    // -----------------------------

    public static CreateBookingCommand ToCommand(this CreateBookingRequest request, Guid currentUserId)
    {
        var clientId = request.ClientId ?? currentUserId;

        return new CreateBookingCommand(
            request.RoomId,
            clientId,
            request.Start,
            request.End,
            request.Comments
            );
    }

    public static UpdateBookingCommand ToCommand(this UpdateBookingRequest request, Guid clientId)
    {
        return new UpdateBookingCommand(
            request.Id,
            request.RoomId,
            clientId,            // viene del booking existente
            request.Start,
            request.End,
            request.Comments
        );
    }

    public static UpdateBookingDatesCommand ToCommand(this UpdateBookingDatesRequest request)
    {
        return new UpdateBookingDatesCommand(
            request.Id,
            request.Start,
            request.End
        );
    }

    public static UpdateBookingCommentsCommand ToCommand(this UpdateBookingCommentsRequest request)
    {
        return new UpdateBookingCommentsCommand(
            request.Id,
            request.Comments
        );
    }

    // -----------------------------
    // DTO → RESPONSE (Salida)
    // -----------------------------

    public static BookingResponse ToResponse(this BookingDto dto)
    {
        return new BookingResponse
        {
            Id = dto.Id,
            RoomId = dto.RoomId,
            ClientId = dto.ClientId,
            CreatedByUserId = dto.CreatedByUserId,
            Start = dto.Start,
            End = dto.End,
            Comments = dto.Comments,
            CreatedAt = dto.CreatedAt,
            Status = dto.Status,
            RoomName = dto.RoomName,
            ClientFullName = dto.ClientFullName
        };
    }

    // -----------------------------
    // ENTITY → RESPONSE (Salida)
    // -----------------------------

    public static BookingResponse ToResponse(this Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            ClientId = booking.ClientId,
            CreatedByUserId = booking.CreatedByUserId,
            Start = booking.DateRange.Start,
            End = booking.DateRange.End,
            Comments = booking.Comments,
            CreatedAt = booking.CreatedAt,
            Status = booking.Status.ToString(),
            RoomName = booking.Room.Name,
            ClientFullName = booking.Client.FullName
        };
    }
}
