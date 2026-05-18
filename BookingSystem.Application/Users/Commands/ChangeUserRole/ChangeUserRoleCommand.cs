using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid UserId, string NewRole) : IRequest<UserDto>;
