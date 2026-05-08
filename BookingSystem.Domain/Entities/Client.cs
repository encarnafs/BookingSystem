using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PhoneNumber PhoneNumber { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; } = false;

    // Constructor privado para EF Core
    private Client() { }

    // Constructor principal
    public Client(string fullName, Email email, PhoneNumber phoneNumber)
    {
        FullName = NormalizeName(fullName);
        Email = email ?? throw new InvalidClientStateException("El email no puede ser nulo.");
        PhoneNumber = phoneNumber ?? throw new InvalidClientStateException("El teléfono no puede ser nulo.");
               
        Id = Guid.NewGuid();
        IsActive = true;
        IsDeleted = false;
    }

    public void UpdateName(string newName)
    {
        FullName = NormalizeName(newName);
    }

    public void UpdateContactInfo(Email email, PhoneNumber phoneNumber)
    {
        Email = email ?? throw new InvalidClientStateException("El email no puede ser nulo.");
        PhoneNumber = phoneNumber ?? throw new InvalidClientStateException("El teléfono no puede ser nulo.");
    }

    public void Update(string fullName, Email email, PhoneNumber phoneNumber)
    {
        UpdateName(fullName);
        UpdateContactInfo(email, phoneNumber);
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidClientNameException("El nombre del cliente no puede estar vacío.");

        var normalized = name.Trim();

        if (normalized.Length < 2)
            throw new InvalidClientNameException("El nombre del cliente es demasiado corto.");

        return normalized;
    }

    public void Disable()
    {
        IsActive = false;
    }

    public void MarkAsDeleted()
    {
        IsActive = false;
        IsDeleted = true;
    }
}


