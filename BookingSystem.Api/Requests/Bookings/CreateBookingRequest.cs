namespace BookingSystem.Api.Requests.Bookings;

public record CreateBookingRequest(
    Guid RoomId,
    Guid ClientId,
    DateTime Start,
    DateTime End,
    string? Comments
);
