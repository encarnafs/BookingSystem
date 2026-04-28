using BookingSystem.Api.Mappers;
using BookingSystem.Api.Responses.Rooms;
using BookingSystem.Application.Rooms.Dtos;

namespace BookingSystem.Api.Mappers;
public static class RoomAvailabilityMapper
{
    public static RoomAvailabilityResponse ToResponse(this RoomAvailabilityDto dto)
    {
        return new RoomAvailabilityResponse
        {
            IsAvailable = dto.IsAvailable,
            ConflictingBookings = dto.ConflictingBookings
                .Select(b => b.ToResponse())
                .ToList()
        };
    }
}
