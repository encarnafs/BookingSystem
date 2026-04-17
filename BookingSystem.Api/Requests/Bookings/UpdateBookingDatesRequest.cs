namespace BookingSystem.Api.Requests.Bookings;

public record UpdateBookingDatesRequest(
    DateTime Start,
    DateTime End
);
