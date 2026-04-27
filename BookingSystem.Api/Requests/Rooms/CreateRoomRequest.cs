namespace BookingSystem.Api.Requests.Rooms;

public class CreateRoomRequest
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Description { get; set; } = null!;
}
//No se incluye IsActive porque se asume que al crear una sala, esta estará activa por defecto.