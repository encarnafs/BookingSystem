using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.UpdateClient;

public class UpdateClientHandler : IRequestHandler<UpdateClientCommand, Guid>
{
    private readonly IClientRepository _clientRepository;

    public UpdateClientHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Guid> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (client is null)
            throw new NotFoundException("Client", request.Id);

        client.Update(request.FullName, request.Email, request.Phone);

        await _clientRepository.UpdateAsync(client, cancellationToken);

        return client.Id;
    }
}
