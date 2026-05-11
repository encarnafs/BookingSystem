namespace BookingSystem.Api.Responses.Users;

/// <summary>
/// Información pública de un usuario dentro del sistema.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre de usuario asignado al usuario.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Rol asignado al usuario dentro del sistema (por ejemplo: Admin o Client).
    /// </summary>
    public string Role { get; set; } = default!;
}
