namespace BookingSystem.Api.Requests.Bookings;

/// <summary>
/// Datos necesarios para actualizar las fechas de una reserva existente.
/// </summary>
public record UpdateBookingDatesRequest(
    Guid Id,
    DateTime Start,
    DateTime End
);
