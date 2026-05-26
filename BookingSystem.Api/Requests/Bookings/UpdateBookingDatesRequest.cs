namespace BookingSystem.Api.Requests.Bookings;

/// <summary>
/// Datos necesarios para actualizar las fechas de una reserva existente.
/// </summary>
public record UpdateBookingDatesRequest(
    DateTime Start,
    DateTime End
);
