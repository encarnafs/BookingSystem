using BookingSystem.Application.Clients.Events;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.DeleteClient;

public class DeleteClientHandler : IRequestHandler<DeleteClientCommand, Unit>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DeleteClientHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Client", request.ClientId);

        if (client.IsDeleted)
            return Unit.Value; // ya está eliminado

        var oldValues = new
        {
            client.IsActive,
            client.IsDeleted
        };

        client.Disable();

        await _clientRepository.UpdateAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new ClientDeletedNotification(
                client.Id,
                oldValues,
                new
                {
                    client.IsActive,
                    client.IsDeleted
                }),
            cancellationToken);

        return Unit.Value;
    }
}
