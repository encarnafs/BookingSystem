using BookingSystem.Domain.Enums;

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
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; }

    public string RoomName { get; init; }
    public string ClientFullName { get; init; }
}
