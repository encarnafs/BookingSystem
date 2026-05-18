namespace BookingSystem.Api.Requests.Users;

/// <summary>
/// Solicitud para cambiar la contraseña de un usuario.
/// </summary>
public class ChangeUserPasswordRequest
{
    /// <summary>
    /// Contraseña actual del usuario.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Nueva contraseña que se asignará al usuario.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}