using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class User
{
    // -----------------------------
    // Propiedades
    // -----------------------------
    public Guid Id { get; private set; }
    public string Username { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

    private User() { }

    // -----------------------------
    // Constructor
    // -----------------------------
    public User(string username, Email email, string role)
    {
        Username = NormalizeUsername(username);
        Email = ValidateEmail(email);
        Role = NormalizeRole(role);

        Id = Guid.NewGuid();
    }

    // -----------------------------
    // Métodos públicos
    // -----------------------------
    public void SetPassword(string passwordHash)
    {
        PasswordHash = ValidatePasswordHash(passwordHash);
    }

    public void ChangePassword(string newHash)
    {
        PasswordHash = ValidatePasswordHash(newHash);
    }
    public void Disable()
    {
        IsActive = false;
    }

    public void Enable()
    {
        IsActive = true;
    }

    public void AssignRole(string newRole)
    {
        Role = NormalizeRole(newRole);
    }

    public void ChangeUsername(string newUsername)
    {
        Username = NormalizeUsername(newUsername);
    }
    public void UpdateEmail(Email newEmail)
    {
        Email = ValidateEmail(newEmail);
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }

    // -----------------------------
    // Métodos privados 
    // -----------------------------
    private static Email ValidateEmail(Email email)
    {
        if (email is null)
            throw new InvalidUserStateException("El email no puede ser nulo.");
        return email;
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

