namespace BookingSystem.Api.Requests.Users;

/// <summary>
/// Datos necesarios para crear un nuevo usuario en el sistema.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Nombre de usuario único que identificará al nuevo usuario.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Correo electrónico del usuario. Debe ser válido y único.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Contraseña que se asociará a la cuenta del usuario.
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// Rol asignado al usuario dentro del sistema (por ejemplo: Admin o Client).
    /// </summary>
    public string Role { get; set; } = default!;
}
