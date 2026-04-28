using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken);

    Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken);
}
