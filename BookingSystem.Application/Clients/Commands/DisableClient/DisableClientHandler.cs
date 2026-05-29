using BookingSystem.Application.Clients.Events;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.DisableClient;

public class DisableClientHandler : IRequestHandler<DisableClientCommand, Unit>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DisableClientHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(DisableClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Client", request.ClientId);

        var oldValues = new
        {
            client.IsActive
        };

        // El dominio valida si ya está deshabilitado
        client.Disable();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new ClientDisabledNotification(
                client.Id,
                oldValues,
                new { client.IsActive }),
            cancellationToken);

        // Auditoría
        await _mediator.Publish(
            new ClientUpdatedNotification(
                client.Id,
                oldValues,
                new { client.IsActive }),
            cancellationToken);

        return Unit.Value;
    }
}
