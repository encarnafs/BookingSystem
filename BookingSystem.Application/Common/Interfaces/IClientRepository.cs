using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;

namespace BookingSystem.Application.Common.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Client>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Client client, CancellationToken cancellationToken);
    Task UpdateAsync(Client client, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken);
}