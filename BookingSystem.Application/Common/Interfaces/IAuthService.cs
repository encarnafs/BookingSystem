using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IAuthService
{
    Task<int> CountUsersAsync();  // Método para contar el número total de usuarios en el sistema. Si es el primero, lo creará como Admin.
    Task<User?> GetUserByEmailAsync(string email);
    Task<Client?> GetClientByEmailAsync(string email);

    Task<User?> CreateUserAsync(User user);
    Task<Client?> CreateClientAsync(Client client);

    Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken);
    Task<Client?> ValidateClientAsync(string email, string password, CancellationToken cancellationToken);
}
