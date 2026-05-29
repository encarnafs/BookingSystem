using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Application.Clients.Events;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Clients.Commands.UpdateClient;

public class UpdateClientHandler : IRequestHandler<UpdateClientCommand, ClientDto>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateClientHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<ClientDto> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener cliente
        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Client", request.Id);

        // 2. Guardar valores antiguos para auditoría
        var oldValues = new
        {
            client.FullName,
            Email = client.Email.Value,
            PhoneNumber = client.PhoneNumber.Value
        };
        
        // 3. Crear Value Objects
        var email = Email.Create(request.Email);
        var phone = PhoneNumber.Create(request.PhoneNumber);

        // 4. Validar duplicados solo si cambian
        if (client.Email.Value != request.Email &&
            await _clientRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException(request.Email);

        if (client.PhoneNumber.Value != request.PhoneNumber &&
            await _clientRepository.ExistsByPhoneAsync(phone, cancellationToken))
            throw new ConflictException(request.PhoneNumber);

        // 5. Actualizar usando el método de dominio
        client.Update(request.FullName, email, phone);

        // 6. Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Publicar evento de auditoría
        await _mediator.Publish(
            new ClientUpdatedNotification(
                client.Id,
                oldValues,
                new
                {
                    client.FullName,
                    Email = client.Email.Value,
                    PhoneNumber = client.PhoneNumber.Value
                }),
            cancellationToken);

        // 8. Devolver DTO
        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email.Value,
            PhoneNumber = client.PhoneNumber.Value
        };
    }
}
