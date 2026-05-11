using BookingSystem.Api.Responses.Bookings;

namespace BookingSystem.Api.Responses.Rooms;

/// <summary>
/// Información sobre la disponibilidad de una sala para un intervalo de tiempo concreto.
/// </summary>
public class RoomAvailabilityResponse
{
    /// <summary>
    /// Indica si la sala está disponible en el intervalo solicitado.
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Lista de reservas que entran en conflicto con la solicitud de disponibilidad.
    /// </summary>
    public List<BookingResponse> ConflictingBookings { get; set; } = new();
}
