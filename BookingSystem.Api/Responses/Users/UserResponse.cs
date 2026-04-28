namespace BookingSystem.Api.Responses.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}
