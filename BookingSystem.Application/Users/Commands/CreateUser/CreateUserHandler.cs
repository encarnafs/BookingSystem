using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Application.Users.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CreateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1️. Crear VO Email
        var email = Email.Create(request.Email);

        // 2️. Validar duplicados
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException("El email ya está registrado.");

        if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
            throw new ConflictException("El nombre de usuario ya existe.");

        // 3️. Crear entidad User sin contraseña
        var user = new User(
            request.Username,
            email,
            request.Role // "Admin" o "User"
        );

        // 4️. Hashear contraseña
        var hashedPassword = _passwordHasher.HashPassword(user, request.Password);
        user.SetPassword(hashedPassword);

        // 5️. Persistir
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6️. Publicar evento
        await _mediator.Publish(new UserCreatedNotification(user.Id), cancellationToken);

        // 7️. Devolver DTO
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }
}

