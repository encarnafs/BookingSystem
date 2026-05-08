namespace BookingSystem.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }          // Quién hizo la acción
    public string Action { get; set; } = "";  // Ej: "UserRoleChanged"
    public string Entity { get; set; } = "";  // Ej: "User"
    public Guid EntityId { get; set; }        // Ej: ID del usuario afectado

    public string? OldValues { get; set; }    // JSON con valores anteriores
    public string? NewValues { get; set; }    // JSON con valores nuevos

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
