namespace BookingSystem.Api.Requests.Auth;

/// <summary>
/// Datos necesarios para registrar un nuevo usuario o cliente en el sistema.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Nombre de usuario único (solo para registro de usuario).
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Nombre completo del cliente (solo para registro de cliente).
    /// </summary>
    public string FullName { get; set; } = default!;

    /// <summary>
    /// Correo electrónico del usuario o cliente.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Número de teléfono del cliente (solo para registro de cliente).
    /// </summary>
    public string PhoneNumber { get; set; } = default!;

    /// <summary>
    /// Contraseña que se asociará a la cuenta.
    /// </summary>
    public string Password { get; set; } = default!;
}

