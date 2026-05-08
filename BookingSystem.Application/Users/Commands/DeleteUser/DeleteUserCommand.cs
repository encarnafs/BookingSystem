using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}
