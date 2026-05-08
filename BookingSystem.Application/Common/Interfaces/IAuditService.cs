namespace BookingSystem.Application.Common.Interfaces;

public interface IAuditService
{
    Task AddEntryAsync(
        Guid userId,
        string action,
        string entity,
        Guid entityId,
        object? oldValues,
        object? newValues);
}
