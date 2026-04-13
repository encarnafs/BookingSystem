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
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la sala NO puede ser vacío");

        if (capacity <= 0)
            throw new ArgumentException("La capacidad debe ser mayor que cero");

        Id = Guid.NewGuid();
        Name = name;
        Capacity = capacity;
        Description = description;
        IsActive = true;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("El nombre de la sala NO puede ser vacío");

        Name = newName;
    }

    public void UpdateCapacity(int newCapacity)
    {
        if (newCapacity <= 0)
            throw new ArgumentException("La capacidad debe ser mayor que cero");

        Capacity = newCapacity;
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("La sala ya está activa");

        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La sala ya está inactiva");

        IsActive = false;
    }
}
