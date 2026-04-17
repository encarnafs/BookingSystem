using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    // Temporal: devuelve Guid.Empty hasta que implementes auth
    public Guid? UserId => Guid.Empty;
}
