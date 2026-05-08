using MediatR;
using BookingSystem.Application.Users.Dtos;

namespace BookingSystem.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
