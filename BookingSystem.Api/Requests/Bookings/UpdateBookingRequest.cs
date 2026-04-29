
namespace BookingSystem.Api.Requests.Bookings;

public class UpdateBookingRequest
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    // El cliente no debe poder enviarlo
    //public Guid ClientId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Comments { get; set; }
}
