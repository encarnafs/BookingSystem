
using BookingSystem.Domain.Exceptions;

namespace BookingSystem.Domain.Exceptions;
public class InvalidUserPasswordException : DomainException
{
    public InvalidUserPasswordException(string message) : base(message) { }
}