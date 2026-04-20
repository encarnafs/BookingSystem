using BookingSystem.Api.Responses.Bookings;
using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Domain.Entities;

namespace BookingSystem.Api.Mappers;

public static class BookingMapper
{
    // Mapeo desde BookingDto (lo que devuelven tus queries)
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
            Comments = dto.Comments
        };
    }

    // Mapeo desde Booking (entidad del dominio)
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
            Comments = booking.Comments
        };
    }
}
