
namespace BookingSystem.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(object domainEvent);
}
