using System.Security.Claims;
using BookingSystem.Application.Common.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BookingSystem.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }
    public string? Email { get; }
    public string? Role { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (Guid.TryParse(userIdClaim, out var id))
                UserId = id;

            Email = emailClaim;
            Role = roleClaim;
        }
    }
}
