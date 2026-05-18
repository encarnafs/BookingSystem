using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<UserDto>;