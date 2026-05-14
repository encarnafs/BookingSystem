namespace BookingSystem.Application.Common.Interfaces;

public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string password);
    bool Verify(string password, string hashedPassword, string salt);
}

