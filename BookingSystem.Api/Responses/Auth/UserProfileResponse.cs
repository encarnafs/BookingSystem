namespace BookingSystem.Api.Responses.Auth;

/// <summary>
/// Información pública del perfil del usuario autenticado.
/// </summary>
public class UserProfileResponse
{
    /// <summary>
    /// Identificador único del usuario. Puede ser nulo si el perfil no está completamente configurado.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Correo electrónico asociado al usuario autenticado.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Rol asignado al usuario dentro del sistema (por ejemplo: Admin o Client).
    /// </summary>
    public string Role { get; set; } = default!;
}
