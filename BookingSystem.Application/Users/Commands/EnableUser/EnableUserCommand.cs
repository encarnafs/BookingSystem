using BookingSystem.Application.Users.Dtos;
using MediatR;


namespace BookingSystem.Application.Users.Commands.EnableUser;

public record EnableUserCommand(Guid UserId) : IRequest<UserDto>;
