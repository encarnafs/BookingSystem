using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using MediatR;

namespace BookingSystem.Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IPasswordHasher _passwordHasher;

    public ChangeUserPasswordHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new Exception($"Usuario con ID {request.UserId} no encontrado.");

        // Validar contraseña actual
        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            throw new Exception("La contraseña actual no es correcta.");

        // Generar nuevo hash + salt
        var (newHash, newSalt) = _passwordHasher.Hash(request.NewPassword);

        var oldValues = new { PasswordHash = user.PasswordHash };
        var newValues = new { PasswordHash = newHash };

        // Actualizar contraseña en la entidad
        user.ChangePassword(newHash, newSalt);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publicar eventos
        await _mediator.Publish(new UserPasswordChangedNotification(user.Id), cancellationToken);
        await _mediator.Publish(new UserUpdatedNotification(user.Id, oldValues, newValues), cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }
}

