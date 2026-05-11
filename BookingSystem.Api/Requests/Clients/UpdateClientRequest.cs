namespace BookingSystem.Api.Requests.Clients;

/// <summary>
/// Datos necesarios para actualizar la información de un cliente existente.
/// </summary>
public class UpdateClientRequest
{
    /// <summary>
    /// Nombre completo del cliente.
    /// </summary>
    public string FullName { get; set; } = default!;

    /// <summary>
    /// Correo electrónico del cliente. Debe ser válido y único.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Número de teléfono de contacto del cliente.
    /// </summary>
    public string Phone { get; set; } = default!;
}
