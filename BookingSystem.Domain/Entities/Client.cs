using BookingSystem.Domain.ValueObjects;
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Entities;

public class Client
{
    // -----------------------------
    // Propiedades
    // -----------------------------
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PhoneNumber PhoneNumber { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public string Role { get; private set; } = "Client";

    // Autenticación
    public string PasswordHash { get; private set; } = default!;

    // Auditoría
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }

    // Constructor privado para EF Core
    private Client() { }

    // -----------------------------
    // Constructor principal (sin password)
    // -----------------------------
    public Client(string fullName, Email email, PhoneNumber phoneNumber)
    {
        FullName = NormalizeName(fullName);
        Email = ValidateEmail(email);
        PhoneNumber = ValidatePhoneNumber(phoneNumber);

        Id = Guid.NewGuid();
        IsActive = true;
        IsDeleted = false;
        Role = "Client";
        CreatedAt = DateTime.UtcNow;
    }

    // -----------------------------
    // Métodos públicos
    // -----------------------------
    public void SetPassword(string passwordHash)
    {
        PasswordHash = ValidatePasswordHash(passwordHash);
    }

    public void SetCreatedBy(Guid createdByUserId)
    {
        CreatedByUserId = createdByUserId;
    }

    public void Update(string fullName, Email email, PhoneNumber phoneNumber)
    {
        UpdateName(fullName);
        UpdateContactInfo(email, phoneNumber);
    }

    public void UpdateName(string newName)
    {
        FullName = NormalizeName(newName);
    }

    public void UpdateContactInfo(Email email, PhoneNumber phoneNumber)
    {
        Email = ValidateEmail(email);
        PhoneNumber = ValidatePhoneNumber(phoneNumber);
    }

    public void Disable()
    {
        if (!IsActive)
            throw new InvalidClientStateException("Inactive");

        IsActive = false;
    }

    public void MarkAsDeleted()
    {
        if (IsDeleted)
            throw new InvalidClientStateException("Deleted");

        IsActive = false;
        IsDeleted = true;
    }

    // -----------------------------
    // Métodos privados (helpers)
    // -----------------------------
    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidClientNameException(name);

        var normalized = name.Trim();

        if (normalized.Length < 2)
            throw new InvalidClientNameException(normalized);

        return normalized;
    }

    private static Email ValidateEmail(Email email)
    {
        if (email is null)
            throw new InvalidClientStateException("null");
        return email;
    }

    private static PhoneNumber ValidatePhoneNumber(PhoneNumber phoneNumber)
    {
        if (phoneNumber is null)
            throw new InvalidClientStateException("null");
        return phoneNumber;
    }

    private static string ValidatePasswordHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new InvalidClientStateException(hash);

        if (hash.Length < 20)
            throw new InvalidClientStateException(hash);

        return hash;
    }
}