using BookingSystem.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace BookingSystem.Domain.ValueObjects;
public sealed class PhoneNumber
{
    // Acepta números internacionales con +, espacios y guiones
    private static readonly Regex PhoneRegex =
        new(@"^\+?[0-9\s\-]{6,20}$", RegexOptions.Compiled);
    public string Value { get; private set; } = default!;

    private PhoneNumber() { }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPhoneNumberException("El número de teléfono no puede estar vacío.");

        var normalized = Normalize(value);

        if (!PhoneRegex.IsMatch(normalized))
            throw new InvalidPhoneNumberException($"El número '{value}' no es válido.");

        Value = normalized;
    }

    private static string Normalize(string input)
    {
        // Elimina espacios y guiones
        return input.Replace(" ", "").Replace("-", "");
    }

    public override string ToString() => Value;
}