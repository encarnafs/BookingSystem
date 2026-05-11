namespace BookingSystem.Api.Requests.Bookings;

/// <summary>
/// Datos necesarios para actualizar una reserva existente.
/// </summary>
public class UpdateBookingRequest
{
    /// <summary>
    /// Identificador único de la reserva que se desea actualizar.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador de la habitación asociada a la reserva.
    /// </summary>
    public Guid RoomId { get; set; }

    // El cliente no debe poder enviarlo
    //public Guid ClientId { get; set; }

    /// <summary>
    /// Nueva fecha y hora de inicio de la reserva.
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Nueva fecha y hora de fin de la reserva.
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Comentarios opcionales asociados a la reserva.
    /// </summary>
    public string? Comments { get; set; }
}
