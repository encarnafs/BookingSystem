namespace BookingSystem.Api.Requests.Bookings;

/// <summary>
/// Datos necesarios para actualizar los comentarios de una reserva existente.
/// </summary>
public record UpdateBookingCommentsRequest(
    Guid Id,
    string? Comments
);
