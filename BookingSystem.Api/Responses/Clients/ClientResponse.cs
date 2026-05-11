namespace BookingSystem.Api.Responses.Clients;

/// <summary>
/// Información pública de un cliente dentro del sistema.
/// </summary>
public class ClientResponse
{
    /// <summary>
    /// Identificador único del cliente.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre completo del cliente.
    /// </summary>
    public string FullName { get; set; } = default!;

    /// <summary>
    /// Correo electrónico del cliente.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Número de teléfono del cliente.
    /// </summary>
    public string Phone { get; set; } = default!;
}
