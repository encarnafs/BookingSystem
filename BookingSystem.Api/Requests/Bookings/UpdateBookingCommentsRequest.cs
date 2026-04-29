namespace BookingSystem.Api.Requests.Bookings;

public record UpdateBookingCommentsRequest(
    Guid Id,
    string? Comments
);
