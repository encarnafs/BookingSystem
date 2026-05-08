using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
