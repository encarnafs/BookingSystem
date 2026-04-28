namespace BookingSystem.Application.Rooms.Dtos;

using BookingSystem.Application.Bookings.Dtos;

public class RoomAvailabilityDto
{
    public bool IsAvailable { get; set; }
    public List<BookingDto> ConflictingBookings { get; set; } = new();
}
