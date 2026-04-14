using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
}