namespace BookingSystem.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = default!;

    // Constructor privado para EF Core
    private User() { }

    // Constructor principal
    public User(string username, string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El nombre de usuario NO puede estar vacío");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña NO puede estar vacío");

        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("El rol NO puede estar vacío");

        Id = Guid.NewGuid();
        Username = username;
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
}