using BookingSystem.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace BookingSystem.Domain.ValueObjects;

public sealed class Email
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    public string Value { get; private set; } = default!;

    private Email() { } // Constructor vacío para EF Core

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("El email no puede estar vacío.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new InvalidEmailException($"El email '{value}' no tiene un formato válido.");

        Value = normalized;
    }

    public override string ToString() => Value;

    public static implicit operator Email(string v)
    {
        throw new NotImplementedException();
    }
}
