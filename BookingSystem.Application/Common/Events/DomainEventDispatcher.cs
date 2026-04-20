using BookingSystem.Application.Bookings.Events;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Events;
using MediatR;
using System.Threading.Tasks;

namespace BookingSystem.Application.Common.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Dispatch(object domainEvent)
    {
        var applicationEvent = ToApplicationEvent(domainEvent);

        // Publicamos el evento de aplicación con MediatR
        await _mediator.Publish(applicationEvent);
    }

    private INotification ToApplicationEvent(object domainEvent)
    {
        return domainEvent switch
        {
            BookingCreatedEvent e => new BookingCreatedNotification(e.BookingId),
            _ => throw new NotImplementedException(
                $"No existe un mapeo para el Domain Event {domainEvent.GetType().Name}")
        };
    }

}
