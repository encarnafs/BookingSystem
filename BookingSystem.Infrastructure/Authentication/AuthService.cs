using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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

    // -----------------------------
    // 1. Buscar usuario por email
    // -----------------------------
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    // -----------------------------
    // 2. Buscar cliente por email
    // -----------------------------
    public async Task<Client?> GetClientByEmailAsync(string email)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Email.Value == email);
    }

    // -----------------------------
    // 3. Crear usuario
    // -----------------------------
    public async Task<User?> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // -----------------------------
    // 4. Crear cliente
    // -----------------------------
    public async Task<Client?> CreateClientAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    // -----------------------------
    // 5. Hash de contraseña
    // -----------------------------
    public (string Hash, string Salt) HashPassword(string password)
    {
        const int iterations = 100000;
        const int saltSize = 16;
        const int keySize = 32;

        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[saltSize];
        rng.GetBytes(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var keyBytes = pbkdf2.GetBytes(keySize);

        var salt = Convert.ToBase64String(saltBytes);
        var hash = Convert.ToBase64String(keyBytes);

        return (hash, salt);
    }

    // -----------------------------
    // 6. Validar usuario (login)
    // -----------------------------
    public async Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

        if (user is null)
            return null;

        if (!VerifyPassword(user.PasswordHash, user.PasswordSalt, password))
            return null;

        return user;
    }

    // -----------------------------
    // 7. Verificar contraseña
    // -----------------------------
    private static bool VerifyPassword(string storedHash, string storedSalt, string password)
    {
        const int iterations = 100000;
        const int keySize = 32;

        var saltBytes = Convert.FromBase64String(storedSalt);
        var storedKeyBytes = Convert.FromBase64String(storedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var computedKey = pbkdf2.GetBytes(keySize);

        return computedKey.SequenceEqual(storedKeyBytes);
    }
}