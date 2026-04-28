using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken: cancellationToken);

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    public Task UpdateAsync(Client client, CancellationToken cancellationToken)
    {
        _context.Clients.Update(client);
        return Task.CompletedTask;
    }

    public async Task<List<Client>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Clients
            .ToListAsync(cancellationToken);
    }

}
