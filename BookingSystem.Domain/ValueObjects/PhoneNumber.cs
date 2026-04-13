namespace BookingSystem.Domain.ValueObjects;

public sealed class PhoneNumber
{
    public string Value { get; private set; } = default!;

    private PhoneNumber() { }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone Number no puede estar vacío");

        if (value.Length < 6)
            throw new ArgumentException("Phone number es demasiado corto");

        Value = value;
    }

    public override string ToString() => Value;
}