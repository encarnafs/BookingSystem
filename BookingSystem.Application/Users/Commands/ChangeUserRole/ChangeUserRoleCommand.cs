using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;
}
