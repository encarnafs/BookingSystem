using BookingSystem.Api.Responses.Bookings;

namespace BookingSystem.Api.Responses.Rooms;

public class RoomAvailabilityResponse
{
    public bool IsAvailable { get; set; }
    public List<BookingResponse> ConflictingBookings { get; set; } = new();
}
