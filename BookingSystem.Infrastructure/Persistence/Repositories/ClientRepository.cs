using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
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
    {
        return await _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Client>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    public Task UpdateAsync(Client client, CancellationToken cancellationToken)
    {
        _context.Clients.Update(client);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        var normalized = email.Value;

        return await _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .AnyAsync(c => c.Email.Value == normalized, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken)
    {
        var normalized = phone.Value;

        return await _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .AnyAsync(c => c.PhoneNumber.Value == normalized, cancellationToken);
    }
}
