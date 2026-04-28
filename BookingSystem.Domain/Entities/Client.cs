using BookingSystem.Domain.ValueObjects;

namespace BookingSystem.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PhoneNumber PhoneNumber { get; private set; } = default!;

    // Constructor privado para EF Core
    private Client() { }

    // Constructor principal
    public Client(string fullName, Email email, PhoneNumber phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("El nombre del cliente NO puede estar vacío");

        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("El nombre del cliente NO puede estar vacío");

        FullName = newName;
    }

    public void UpdateContactInfo(Email email, PhoneNumber phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void Update(string fullName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("El nombre NO puede estar vacío");

        FullName = fullName;
        Email = new Email(email);
        PhoneNumber = new PhoneNumber(phoneNumber);
    }


}


