using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BookingSystem.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32;  // 256 bits
    private const int Iterations = 100000;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken)
    {
        var emailVO = Email.Create(email);

        if (await _context.Users.AnyAsync(u => u.Email.Value == emailVO.Value, cancellationToken))
            throw new Exception("El email ya está registrado.");

        var passwordHash = HashPassword(password);

        var user = new User(username, emailVO, passwordHash, role: "User");

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

        if (user is null)
            return null;

        if (!VerifyPassword(user.PasswordHash, password))
            return null;

        return user;
    }

    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    private static bool VerifyPassword(string storedHash, string password)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 3)
            return false;

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var keyToCheck = pbkdf2.GetBytes(KeySize);

        return keyToCheck.SequenceEqual(key);
    }
}

