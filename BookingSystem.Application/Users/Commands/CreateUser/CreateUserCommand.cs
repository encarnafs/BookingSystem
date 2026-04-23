using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    string Role
) : IRequest<UserDto>;
