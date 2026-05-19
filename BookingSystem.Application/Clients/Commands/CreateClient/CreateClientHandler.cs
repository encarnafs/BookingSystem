using BookingSystem.Application.Clients.Commands.CreateClient;
using BookingSystem.Application.Clients.Dtos;
using BookingSystem.Application.Clients.Events;
using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Application.Clients.Commands.CreateClient;

public class CreateClientHandler : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IPasswordHasher<Client> _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public CreateClientHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IPasswordHasher<Client> passwordHasher,
        ICurrentUserService currentUserService)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // 1️⃣ Crear Value Objects primero
        var email = Email.Create(request.Email);
        var phone = PhoneNumber.Create(request.PhoneNumber);

        // 2️⃣ Validar duplicados
        if (await _clientRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException("Ya existe un cliente con este email.");

        if (await _clientRepository.ExistsByPhoneAsync(phone, cancellationToken))
            throw new ConflictException("Ya existe un cliente con este teléfono.");

        // 3️⃣ Crear entidad Client (sin contraseña)
        var client = new Client(request.FullName, email, phone);

        // 4️⃣ Hashear contraseña con el estándar
        var hashedPassword = _passwordHasher.HashPassword(client, request.Password);
        client.SetPassword(hashedPassword);

        // 5️⃣ Asignar el Admin como creador
        client.SetCreatedBy(_currentUserService.UserId ?? client.Id);

        // 6️⃣ Persistir
        await _clientRepository.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7️⃣ Publicar evento
        await _mediator.Publish(new ClientCreatedNotification(client.Id), cancellationToken);

        // 8️⃣ Devolver DTO
        return new ClientDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email.Value,
            PhoneNumber = client.PhoneNumber.Value
        };
    }
}

