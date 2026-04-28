using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Application.Common.Interfaces;
using MediatR;

namespace BookingSystem.Application.Clients.Queries.GetAllClients;

public class GetAllClientsHandler : IRequestHandler<GetAllClientsQuery, List<ClientDto>>
{
    private readonly IClientRepository _clientRepository;

    public GetAllClientsHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _clientRepository.GetAllAsync(cancellationToken);

        return clients.Select(c => new ClientDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email.Value,
            PhoneNumber = c.PhoneNumber.Value
        }).ToList();
    }
}
