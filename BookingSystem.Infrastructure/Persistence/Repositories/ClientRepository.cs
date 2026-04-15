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

    public async Task<Client?> GetByIdAsync(Guid id)
        => await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Client client)
    => await _context.Clients.AddAsync(client);

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }
}
