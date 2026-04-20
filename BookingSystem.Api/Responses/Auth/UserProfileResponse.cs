namespace BookingSystem.Api.Responses.Auth;

public class UserProfileResponse
{
    public string UserId { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
}
