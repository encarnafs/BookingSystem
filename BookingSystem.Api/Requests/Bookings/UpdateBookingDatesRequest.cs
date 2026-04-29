namespace BookingSystem.Api.Requests.Bookings;

public record UpdateBookingDatesRequest(
    Guid Id,
    DateTime Start,
    DateTime End
);
