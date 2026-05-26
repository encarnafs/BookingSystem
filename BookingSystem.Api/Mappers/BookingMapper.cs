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

    // Este método convierte un CreateBookingRequest en un CreateBookingCommand.
    // Se usa inicialización por propiedades porque CreateBookingCommand ahora es una clase mutable.
    // El constructor con parámetros ya no existe (antes era un record posicional).
    // Esto permite asignar ClientId dinámicamente según el usuario autenticado.
    public static CreateBookingCommand ToCommand(this CreateBookingRequest request, Guid currentUserId)
    {
        // Si el request no incluye ClientId (por ejemplo, cuando el rol es Client),
        // se usa el ID del usuario autenticado para evitar que un cliente reserve en nombre de otro.
        // ClientId puede ser nullable (Guid?), mientras que currentUserId es Guid.
        // Se usa HasValue/Value para evitar el error CS0266 y garantizar que el tipo final sea Guid.
        var clientId = request.ClientId.GetValueOrDefault(currentUserId);

        // Inicialización por propiedades (object initializer)
        return new CreateBookingCommand
        {
            RoomId = request.RoomId,
            ClientId = clientId,
            Start = request.Start,
            End = request.End,
            Comments = request.Comments
        };
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

    public static UpdateBookingDatesCommand ToCommand(this UpdateBookingDatesRequest request, Guid id)
    {
        return new UpdateBookingDatesCommand(id, request.Start, request.End);
    }

    public static UpdateBookingCommentsCommand ToCommand(this UpdateBookingCommentsRequest request, Guid id)
    {
        return new UpdateBookingCommentsCommand(
            id,
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
            Status = dto.Status.ToString(),
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
