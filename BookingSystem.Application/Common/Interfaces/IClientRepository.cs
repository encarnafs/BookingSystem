using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Client>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Client client, CancellationToken cancellationToken);
    Task UpdateAsync(Client client, CancellationToken cancellationToken);
}