using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByRoomAsync(Guid roomId);
    Task<IEnumerable<Booking>> GetByClientAsync(Guid clientId);
    Task<IEnumerable<Booking>> GetInDateRangeAsync(DateTime start, DateTime end);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<bool> ExistsOverlappingBookingAsync(Guid roomId, DateTime start, DateTime end);
    Task<int> CountBookingsForClientOnDateAsync(Guid clientId, DateTime date);
}
