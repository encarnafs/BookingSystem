using BookingSystem.Domain.ValueObjects;

namespace BookingSystem.Api.Requests.Bookings;

public class UpdateBookingRequest
{
    public Guid RoomId { get; set; }
    public Guid ClientId { get; set; }
    public DateRange DateRange { get; set; } = default!;
    public string? Comments { get; set; }
}
