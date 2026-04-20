namespace BookingSystem.Api.Responses.Bookings;

public class BookingResponse
{
    public Guid Id { get; init; }
    public Guid RoomId { get; init; }
    public Guid ClientId { get; init; }
    public Guid CreatedByUserId { get; init; }
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public string? Comments { get; init; }
}
