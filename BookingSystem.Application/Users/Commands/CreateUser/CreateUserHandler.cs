using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;
using System.Security.Cryptography;

namespace BookingSystem.Application.Users.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validar duplicados
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new Exception("El email ya está registrado.");

        if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
            throw new Exception("El nombre de usuario ya existe.");

        // Hash temporal
        var passwordHash = HashPassword(request.Password);

        var email = Email.Create(request.Email);

        var user = new User(
            request.Username,
            email,
            passwordHash,
            request.Role
        );

        await _userRepository.AddAsync(user, cancellationToken);

        // Guardar cambios
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publicar evento
        await _mediator.Publish(new UserCreatedNotification(user.Id), cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }

    private static string HashPassword(string password)
    {
        const int iterations = 10000;
        const int saltSize = 16;
        const int keySize = 32;

        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[saltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(keySize);

        return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }
}

