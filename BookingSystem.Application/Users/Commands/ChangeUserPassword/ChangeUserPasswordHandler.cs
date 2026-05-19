using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using BookingSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ChangeUserPasswordHandler(
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

    public async Task<UserDto> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("UserId", request.UserId);

        // 1️⃣ Validar contraseña actual
        var verifyResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.CurrentPassword
        );

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("La contraseña actual no es correcta.");

        // 2️⃣ Generar nuevo hash
        var newHash = _passwordHasher.HashPassword(user, request.NewPassword);

        var oldValues = new { PasswordHash = user.PasswordHash };
        var newValues = new { PasswordHash = newHash };

        // 3️⃣ Actualizar contraseña en la entidad
        user.SetPassword(newHash);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4️⃣ Publicar eventos
        await _mediator.Publish(new UserPasswordChangedNotification(user.Id), cancellationToken);
        await _mediator.Publish(new UserUpdatedNotification(user.Id, oldValues, newValues), cancellationToken);

        // 5️⃣ Devolver DTO
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }
}

