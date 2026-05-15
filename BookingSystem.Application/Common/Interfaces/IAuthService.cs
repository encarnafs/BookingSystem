using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IAuthService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<Client?> GetClientByEmailAsync(string email);

    (string Hash, string Salt) HashPassword(string password);

    Task<User?> CreateUserAsync(User user);
    Task<Client?> CreateClientAsync(Client client);

    Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task<Client?> ValidateClientAsync(string email, string password, CancellationToken cancellationToken);
}
