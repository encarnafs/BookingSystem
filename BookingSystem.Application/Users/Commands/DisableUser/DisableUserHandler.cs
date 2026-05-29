using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using MediatR;
using System.Text.RegularExpressions;

namespace BookingSystem.Application.Users.Commands.DisableUser;

public class DisableUserHandler : IRequestHandler<DisableUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DisableUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        // No permitir desactivar un Admin
        if (user.Role == "Admin")
            throw new ConflictException(user.Role);

        var oldValues = new { IsActive = user.IsActive };

        // Desactivar usuario
        user.Disable();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publicar evento
        await _mediator.Publish(
            new UserDisabledNotification(user.Id),
            cancellationToken);

        // Auditoría
        await _mediator.Publish(
            new UserUpdatedNotification(
                user.Id,
                oldValues,
                new { IsActive = user.IsActive }),
            cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            Role = user.Role
        };
    }
}
