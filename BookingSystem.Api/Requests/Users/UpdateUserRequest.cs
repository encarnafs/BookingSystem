namespace BookingSystem.Api.Requests.Users;

/// <summary>
/// Datos necesarios para actualizar la información de un usuario existente.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Nombre de usuario que identificará al usuario dentro del sistema.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario. Debe ser válido y único.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Rol asignado al usuario dentro del sistema (por ejemplo: Admin o Client).
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
