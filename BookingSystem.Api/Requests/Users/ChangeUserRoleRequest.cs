namespace BookingSystem.Api.Requests.Users;

/// <summary>
/// Solicitud para cambiar el rol de un usuario.
/// </summary>
public class ChangeUserRoleRequest
{
    /// <summary>
    /// Nuevo rol que se asignará al usuario.
    /// </summary>
    public string NewRole { get; set; } = string.Empty;
}