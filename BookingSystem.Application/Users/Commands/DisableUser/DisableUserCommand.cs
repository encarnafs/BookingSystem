using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.DisableUser;

public record DisableUserCommand(Guid UserId) : IRequest<UserDto>;
