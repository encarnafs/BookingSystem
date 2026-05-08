using BookingSystem.Application.Clients.Commands.CreateClient;
using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Application.Clients.Events;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.CreateClient;
public class CreateClientHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateClientHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // 1. Crear Value Objects primero
        var email = Email.Create(request.Email);
        var phone = PhoneNumber.Create(request.PhoneNumber);

        // 2. Validar duplicados usando VO
        if (await _clientRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new Exception("Ya existe un cliente con este email.");

        if (await _clientRepository.ExistsByPhoneAsync(phone, cancellationToken))
            throw new Exception("Ya existe un cliente con este teléfono.");

        // 3. Crear entidad
        var client = new Client(request.FullName, email, phone);

        // 4. Persistir
        await _clientRepository.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Publicar evento
        await _mediator.Publish(new ClientCreatedNotification(client.Id), cancellationToken);

        // 6. Devolver DTO
        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email.Value,
            PhoneNumber = client.PhoneNumber.Value
        };
    }

}

