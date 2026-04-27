namespace BookingSystem.Api.Responses.Rooms;
public class RoomResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int Capacity { get; init; }
    public string Description { get; init; } = null!;
    public bool IsActive { get; init; }
}
