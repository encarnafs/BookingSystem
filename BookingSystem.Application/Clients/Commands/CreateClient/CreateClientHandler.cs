using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.CreateClient;

public class CreateClientHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _clientRepository;

    public CreateClientHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var phone = new PhoneNumber(request.PhoneNumber);

        var client = new Client(request.FullName, email, phone);

        await _clientRepository.AddAsync(client, cancellationToken);

        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email.Value,
            PhoneNumber = client.PhoneNumber.Value
        };
    }
}
