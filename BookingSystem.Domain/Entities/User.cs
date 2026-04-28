namespace BookingSystem.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = default!;

    private User() { }

    public User(string username, string email, string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El nombre de usuario NO puede estar vacío");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email NO puede estar vacío");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña NO puede estar vacío");

        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("El rol NO puede estar vacío");

        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("El hash de la contraseña NO puede estar vacío");

        PasswordHash = newPasswordHash;
    }

    public void AssignRole(string newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole))
            throw new ArgumentException("El rol NO puede estar vacío");

        Role = newRole;
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("El email NO puede estar vacío");

        Email = newEmail;
    }
}
