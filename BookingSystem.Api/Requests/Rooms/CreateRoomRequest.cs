namespace BookingSystem.Api.Requests.Rooms;

/// <summary>
/// Datos necesarios para crear una nueva sala en el sistema.
/// </summary>
public class CreateRoomRequest
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
}
//No se incluye IsActive porque se asume que al crear una sala, esta estará activa por defecto.
