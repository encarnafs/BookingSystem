using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken: cancellationToken);

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task AddAsync(User user, CancellationToken cancellationToken)
    { 
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken: cancellationToken);
}
