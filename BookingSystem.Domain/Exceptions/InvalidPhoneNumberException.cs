namespace BookingSystem.Domain.Exceptions;

public class InvalidPhoneNumberException : DomainException
{
    public InvalidPhoneNumberException(string phoneNumber)
        : base($"El número de teléfono '{phoneNumber}' no es válido.")
    {
    }
}