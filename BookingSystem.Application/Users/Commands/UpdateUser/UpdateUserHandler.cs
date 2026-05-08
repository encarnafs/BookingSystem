using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.ValueObjects;
using MediatR;

namespace BookingSystem.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new Exception($"Usuario con ID {request.Id} no encontrado.");

        // Validar duplicado de email si se cambia
        if (user.Email.Value != request.Email &&
            await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new Exception("El email ya está registrado.");
        }

        var oldValues = new
        {
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };

        // Actualizar campos
        user.Username = request.Username;
        user.Email = new Email(request.Email);
        user.Role = request.Role;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publicar evento si el rol cambió
        if (oldValues.Role != user.Role)
        {
            await _mediator.Publish(new UserRoleChangedNotification(
                user.Id,
                oldValues.Role,
                user.Role), cancellationToken);
        }

        // Auditoría general del cambio
        await _mediator.Publish(new UserUpdatedNotification(
            user.Id,
            oldValues,
            new
            {
                Username = user.Username,
                Email = user.Email.Value,
                Role = user.Role
            }), cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }
}

