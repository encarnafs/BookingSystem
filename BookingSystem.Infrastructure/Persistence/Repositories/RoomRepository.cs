using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
        => await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(Room room)
    => await _context.Rooms.AddAsync(room);

    public async Task<IEnumerable<Room>> GetAllAsync()
        => await _context.Rooms.ToListAsync();

    public async Task UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }
}
