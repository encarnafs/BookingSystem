namespace BookingSystem.Api.Responses.Rooms;

/// <summary>
/// Información básica de disponibilidad visible para clientes.
/// </summary>
public class RoomAvailabilityClientResponse
{
    /// <summary>
    /// Indica si la sala está disponible en el intervalo solicitado.
    /// </summary>
    public bool IsAvailable { get; set; }
}