namespace BookingSystem.Api.Requests.Rooms;

/// <summary>
/// Datos necesarios para actualizar la información de una sala existente.
/// </summary>
public class UpdateRoomRequest
{
    /// <summary>
    /// Nombre identificativo de la sala.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Capacidad máxima de personas que puede albergar la sala.
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Descripción de la sala, incluyendo detalles relevantes para su uso.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Indica si la sala está activa y disponible para reservas.
    /// </summary>
    public bool IsActive { get; set; }
}
