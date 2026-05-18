using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.ChangeUserPassword;

public record ChangeUserPasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<UserDto>;