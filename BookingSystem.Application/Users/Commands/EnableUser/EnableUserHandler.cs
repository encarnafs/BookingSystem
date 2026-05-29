using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Users.Commands.EnableUser;

public class EnableUserCommandHandler
    : IRequestHandler<EnableUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public EnableUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(EnableUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var oldValues = new { IsActive = user.IsActive };

        // Activar usuario (el dominio valida estado)
        user.Enable();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new UserEnabledNotification(user.Id), cancellationToken);

        // Auditoría (simétrico a DisableUser)
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

