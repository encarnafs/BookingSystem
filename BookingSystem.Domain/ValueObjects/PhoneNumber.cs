using BookingSystem.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace BookingSystem.Domain.ValueObjects;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    // Solo números, con + opcional al inicio, entre 6 y 20 dígitos
    private static readonly Regex PhoneRegex =
        new(@"^\+?[0-9]{6,20}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPhoneNumberException("El número de teléfono no puede estar vacío.");

        var normalized = Normalize(value);

        if (!PhoneRegex.IsMatch(normalized))
            throw new InvalidPhoneNumberException($"El número '{value}' no es válido.");

        return new PhoneNumber(normalized);
    }

    private static string Normalize(string input)
    {
        // Elimina espacios y guiones
        return input.Replace(" ", "").Replace("-", "");
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

    public bool Equals(PhoneNumber? other) =>
        other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}
