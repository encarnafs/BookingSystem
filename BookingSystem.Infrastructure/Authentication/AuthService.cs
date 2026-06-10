using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IPasswordHasher<Client> _clientPasswordHasher;

    public AuthService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, IPasswordHasher<Client> clientPasswordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _clientPasswordHasher = clientPasswordHasher;
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
    // 5. Validar usuario (login)
    // -----------------------------
    public async Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        // 1. Busca el usuario por email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

        // 2. Si el usuario fue Deleted, aquí llegaría null, porque el filtro global de borrado lógico (UserConfiguration.cs) lo excluye automáticamente.
        if (user is null)
            return null;

        // 3. Verificar la contraseña hasheada. Si la contraseña no coincide, se devuelve null.
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        // 4. Si todo es correcto, se devuelve el usuario autenticado
        return user;
    }

    // -----------------------------
    // 6. Validar cliente (login)
    // -----------------------------
    public async Task<Client?> ValidateClientAsync(string email, string password, CancellationToken cancellationToken)
    {
        // 1. Buscar el cliente por email
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email.Value == email, cancellationToken);

        // 2. Si el cliente fue Deleted, aquí llegaría null, porque el filtro global de borrado lógico (ClientConfiguration.cs) lo excluye automáticamente.
        if (client is null)
            return null;

        // 3. Verificar la contraseña hasheada. Si no coincide, se devuelve null.
        var result = _clientPasswordHasher.VerifyHashedPassword(client, client.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        // 4. Si todo es correcto, se devuelve el cliente autenticado
        return client;
    }

    // -----------------------------
    // 7. Contar usuarios
    // -----------------------------
    public async Task<int> CountUsersAsync()
    {
        return await _context.Users.CountAsync();
    }

}