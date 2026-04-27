namespace BookingSystem.Api.Requests.Rooms;

public class UpdateRoomRequest
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}
