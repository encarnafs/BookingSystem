using BookingSystem.Domain.Entities;

namespace BookingSystem.Application.Common.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<IEnumerable<Room>> GetAllAsync();
    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
}