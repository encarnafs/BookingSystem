using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public int Capacity { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Constructor privado para EF Core (si lo usamos)
    private Room() { }

    // Constructor principal
    public Room(string name, int capacity, string? description = null)
    {
        Name = NormalizeName(name);
        Capacity = ValidateCapacity(capacity);
        Description = NormalizeDescription(description);
        IsActive = true;

        Id = Guid.NewGuid();
    }

    public void UpdateName(string newName)
    {
        Name = NormalizeName(newName);
    }

    public void UpdateCapacity(int newCapacity)
    {
        Capacity = ValidateCapacity(newCapacity);
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = NormalizeDescription(newDescription);
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidRoomStateException("La sala ya está activa.");

        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidRoomStateException("La sala ya está inactiva.");

        IsActive = false;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRoomNameException("El nombre de la sala no puede estar vacío.");

        var normalized = name.Trim();

        if (normalized.Length < 2)
            throw new InvalidRoomNameException("El nombre de la sala es demasiado corto.");

        return normalized;
    }

    private static int ValidateCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new InvalidRoomCapacityException("La capacidad debe ser mayor que cero.");

        return capacity;
    }

    private static string? NormalizeDescription(string? description)
    {
        return description?.Trim();
    }
}

