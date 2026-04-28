namespace BookingSystem.Api.Responses.Auth;

public class UserProfileResponse
{
    public Guid? UserId { get; set; }
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}
