using BookingSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;

        Console.WriteLine("=== TOKEN GENERATOR SETTINGS ===");
        Console.WriteLine($"Issuer: {_settings.Issuer}");
        Console.WriteLine($"Audience: {_settings.Audience}");
        Console.WriteLine($"Secret length: {_settings.Secret.Length}");
        Console.WriteLine("====================");
    }

    public string GenerateToken(Guid userId, string email, string username, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim("role", role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: creds);

        Console.WriteLine($"JWT Secret (first 10 chars): {_settings.Secret.Substring(0, 10)}");


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
