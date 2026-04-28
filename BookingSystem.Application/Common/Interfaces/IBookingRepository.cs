using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Booking>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken);
    Task<IEnumerable<Booking>> GetByClientAsync(Guid clientId, CancellationToken cancellationToken);
    Task<IEnumerable<Booking>> GetInDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken);
    Task AddAsync(Booking booking, CancellationToken cancellationToken);
    Task UpdateAsync(Booking booking, CancellationToken cancellationToken);
    Task<bool> ExistsOverlappingBookingAsync(Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken);
    Task<int> CountBookingsForClientOnDateAsync(Guid clientId, DateTime date, CancellationToken cancellationToken);
}
