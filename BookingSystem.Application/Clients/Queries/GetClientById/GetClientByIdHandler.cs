using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Clients.Queries.GetClientById;

public class GetClientByIdHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientRepository _clientRepository;
    public GetClientByIdHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    public async Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);

        // Soy más explícita enviando null en vez de una excepción, ya que el cliente no existe, no es un error del sistema.
        if (client is null) 
            return null;

        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email.Value,
            PhoneNumber = client.PhoneNumber.Value
        };
    }
}
