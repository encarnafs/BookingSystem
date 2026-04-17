using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BookingSystem.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new Exception("El email ya está registrado.");

        var passwordHash = HashPassword(password);

        var user = new User(username, email, passwordHash, role: "User");

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
            return null;

        var passwordHash = HashPassword(password);

        if (user.PasswordHash != passwordHash)
            return null;

        return user;
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
