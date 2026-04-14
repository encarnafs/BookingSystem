namespace BookingSystem.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"{name} con key '{key}' no fue encontrado.")
    {
    }
}
