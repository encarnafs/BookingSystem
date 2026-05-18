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

        if (!client.IsActive)
            return Unit.Value; // ya está deshabilitado, no hacemos nada

        var oldValues = new
        {
            client.IsActive
        };

        client.Disable();

        await _clientRepository.UpdateAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new ClientDisabledNotification(
                client.Id,
                oldValues,
                new { client.IsActive }),
            cancellationToken);

        return Unit.Value;
    }
}
