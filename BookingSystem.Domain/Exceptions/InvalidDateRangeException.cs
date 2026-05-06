using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class InvalidDateRangeException : DomainException
{
    public InvalidDateRangeException(DateTime start, DateTime end)
        : base($"El rango de fechas es inválido: Start={start}, End={end}")
    {
    }
}
