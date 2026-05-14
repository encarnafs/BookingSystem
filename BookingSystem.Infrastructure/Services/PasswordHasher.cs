using BookingSystem.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BookingSystem.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public (string Hash, string Salt) Hash(string password)
    {
        var salt = Guid.NewGuid().ToString("N");
        var hash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password + salt)
            )
        );
        return (hash, salt);
    }

    public bool Verify(string password, string hash, string salt)
    {
        var computedHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(password + salt)
            )
        );
        return computedHash == hash;
    }
}
