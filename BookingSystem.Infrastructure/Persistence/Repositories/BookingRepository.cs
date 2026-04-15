using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
        => await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Booking booking)
        => await _context.Bookings.AddAsync(booking);

    public async Task<IEnumerable<Booking>> GetByRoomAsync(Guid roomId)
        => await _context.Bookings.Where(b => b.RoomId == roomId).ToListAsync();

    public async Task<IEnumerable<Booking>> GetByClientAsync(Guid clientId)
        => await _context.Bookings.Where(b => b.ClientId == clientId).ToListAsync();

    public async Task<IEnumerable<Booking>> GetInDateRangeAsync(DateTime start, DateTime end)
        => await _context.Bookings
            .Where(b => b.DateRange.Start >= start && b.DateRange.End <= end)
            .ToListAsync();

    public async Task<bool> ExistsOverlappingBookingAsync(Guid roomId, DateTime start, DateTime end)
        => await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.DateRange.Start < end &&
            b.DateRange.End > start);

    public async Task<int> CountBookingsForClientOnDateAsync(Guid clientId, DateTime date)
        => await _context.Bookings.CountAsync(b =>
            b.ClientId == clientId &&
            b.DateRange.Start <= date &&
            b.DateRange.End >= date);

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
