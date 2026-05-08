using BookingSystem.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace BookingSystem.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("El email no puede estar vacío.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new InvalidEmailException($"El email '{value}' no tiene un formato válido.");

        return new Email(normalized);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => Equals(obj as Email);

    public bool Equals(Email? other) =>
        other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}
