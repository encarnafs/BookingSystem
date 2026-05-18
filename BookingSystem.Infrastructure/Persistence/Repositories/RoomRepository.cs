using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Persistence.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoomRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Room room, CancellationToken cancellationToken)
        {
            await _dbContext.Rooms.AddAsync(room, cancellationToken);
        }

        public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Rooms
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }
        public async Task<IList<Room>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Rooms
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public Task UpdateAsync(Room room, CancellationToken cancellationToken)
        {
            _dbContext.Rooms.Update(room);
            return Task.CompletedTask;
        }
    }
}
