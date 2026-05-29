using BookingSystem.Application.Common.Exceptions;
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
    private readonly ICurrentUserService _currentUser;

    public DeleteUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdIncludingDeletedAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        // Solo un Admin puede borrar usuarios
        if (_currentUser.Role != "Admin")
            throw new ForbiddenAccessException(_currentUser.Role!);

        // Un Admin no puede borrarse a sí mismo
        if (Guid.TryParse(_currentUser.UserId?.ToString(), out var currentUserId) && currentUserId == user.Id)
            throw new ConflictException(user.Id.ToString());

        // Un Admin no puede borrar a otro Admin
        if (user.Role == "Admin")
            throw new ConflictException(user.Role);

        var oldValues = new { IsDeleted = user.IsDeleted };

        // Soft delete
        user.MarkAsDeleted();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new UserDeletedNotification(user.Id), cancellationToken);

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
