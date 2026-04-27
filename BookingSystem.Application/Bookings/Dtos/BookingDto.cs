namespace BookingSystem.Application.Bookings.Dtos;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid ClientId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Comments { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string RoomName { get; set; } = default!;
    public string ClientFullName { get; set; } = default!;
}