using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;
public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = default!;

    private User() { }

    public User(string username, Email email, string passwordHash, string role)
    {
        Username = NormalizeUsername(username);
        Email = email ?? throw new InvalidUserStateException("El email no puede ser nulo.");
        PasswordHash = ValidatePasswordHash(passwordHash);
        Role = NormalizeRole(role);

        Id = Guid.NewGuid();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = ValidatePasswordHash(newPasswordHash);
    }

    public void AssignRole(string newRole)
    {
        Role = NormalizeRole(newRole);
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail ?? throw new InvalidUserStateException("El email no puede ser nulo.");
    }

    private static string NormalizeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidUserNameException("El nombre de usuario no puede estar vacío.");

        var normalized = username.Trim();

        if (normalized.Length < 3)
            throw new InvalidUserNameException("El nombre de usuario es demasiado corto.");

        return normalized;
    }

    private static string NormalizeRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidUserRoleException("El rol no puede estar vacío.");

        return role.Trim();
    }

    private static string ValidatePasswordHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new InvalidUserPasswordException("El hash de la contraseña no puede estar vacío.");

        if (hash.Length < 20)
            throw new InvalidUserPasswordException("El hash de la contraseña no es válido.");

        return hash;
    }
}
