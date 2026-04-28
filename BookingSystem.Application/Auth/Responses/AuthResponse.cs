namespace BookingSystem.Application.Auth.Responses;

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}
