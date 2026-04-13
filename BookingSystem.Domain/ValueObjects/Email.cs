

namespace BookingSystem.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; private set; } = default!;

    private Email() { } // Constructor vacío para EF Core

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email NO puede estar vacío");

        if (!value.Contains("@"))
            throw new ArgumentException("Formato Email inválido");

        Value = value.Trim().ToLower();
    }

    public override string ToString() => Value;
}
