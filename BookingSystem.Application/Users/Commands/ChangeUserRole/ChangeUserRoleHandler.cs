using BookingSystem.Application.Common.Exceptions;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using MediatR;

namespace BookingSystem.Application.Users.Commands.ChangeUserRole;

public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ChangeUserRoleHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var oldRole = user.Role;

        if (oldRole == request.NewRole)
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                Role = user.Role
            };

        // Actualizar rol
        user.AssignRole(request.NewRole);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publicar evento de cambio de rol
        await _mediator.Publish(
            new UserRoleChangedNotification(user.Id, oldRole, request.NewRole),
            cancellationToken);

        // Auditoría
        await _mediator.Publish(
            new UserUpdatedNotification(
                user.Id,
                new { Role = oldRole },
                new { Role = request.NewRole }),
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
