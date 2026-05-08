using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using BookingSystem.Application.Users.Events;
using MediatR;

namespace BookingSystem.Application.Users.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DeleteUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new Exception($"Usuario con ID {request.UserId} no encontrado.");

        if (user.IsDeleted)
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                Role = user.Role
            };

        var oldValues = new { IsDeleted = user.IsDeleted };

        // Marcar usuario como eliminado
        user.MarkAsDeleted();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publicar evento
        await _mediator.Publish(
            new UserDeletedNotification(user.Id),
            cancellationToken);

        // Auditoría
        await _mediator.Publish(
            new UserUpdatedNotification(
                user.Id,
                oldValues,
                new { IsDeleted = user.IsDeleted }),
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
