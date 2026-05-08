using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.DisableUser;

public class DisableUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}
