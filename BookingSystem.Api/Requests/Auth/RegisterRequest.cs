namespace BookingSystem.Api.Requests.Auth;

/// <summary>
/// Datos necesarios para registrar un nuevo usuario en el sistema.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Nombre de usuario único que identificará al nuevo usuario.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Correo electrónico del usuario. Debe ser válido y no estar registrado previamente.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Contraseña que se asociará a la cuenta del usuario.
    /// </summary>
    public string Password { get; set; } = default!;
}
