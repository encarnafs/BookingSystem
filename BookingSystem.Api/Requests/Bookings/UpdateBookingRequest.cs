
namespace BookingSystem.Api.Requests.Bookings;

public class UpdateBookingRequest
{
    public Guid RoomId { get; set; }
    public Guid ClientId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Comments { get; set; }
}
