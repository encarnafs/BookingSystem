namespace BookingSystem.Api.Requests.Auth;

/// <summary>
/// Datos necesarios para iniciar sesión en el sistema.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Correo electrónico del usuario que intenta autenticarse.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Contraseña asociada al correo electrónico del usuario.
    /// </summary>
    public string Password { get; set; } = default!;
}
