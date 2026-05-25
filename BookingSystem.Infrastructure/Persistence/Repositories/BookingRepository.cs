using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Persistence.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookingRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
        {
            await _dbContext.Bookings.AddAsync(booking, cancellationToken);
        }

        public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Bookings
                .Include(b => b.Room)
                .Include(b => b.Client)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken)
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .Include(b => b.Client)
                .Where(b => b.RoomId == roomId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetByClientAsync(Guid clientId, CancellationToken cancellationToken)
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .Include(b => b.Client)
                .Where(b => b.ClientId == clientId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetInDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .Include(b => b.Client)
                .Where(b => b.DateRange.Start < end && b.DateRange.End > start)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsOverlappingBookingAsync(Guid roomId, DateTime start, DateTime end, Guid? excludeBookingId, CancellationToken cancellationToken)
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .AnyAsync(b =>
                    b.RoomId == roomId &&
                    b.Id != excludeBookingId && // excluir la propia reserva
                    b.DateRange.Start < end &&
                    start < b.DateRange.End,
                    cancellationToken);
        }

        public async Task<int> CountBookingsForClientOnDateAsync(Guid clientId, DateTime date, CancellationToken cancellationToken)
        {
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);
            return await _dbContext.Bookings
                .AsNoTracking()
                .CountAsync(b =>
                    b.ClientId == clientId &&
                    b.DateRange.Start < dayEnd &&
                    b.DateRange.End > dayStart,
                    cancellationToken);
        }

        public  Task UpdateAsync(Booking booking, CancellationToken cancellationToken)
        {
            _dbContext.Bookings.Update(booking);
            return Task.CompletedTask;
        }

        public async Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .Include(b => b.Client)
                .ToListAsync(cancellationToken);
        }

    }
}
