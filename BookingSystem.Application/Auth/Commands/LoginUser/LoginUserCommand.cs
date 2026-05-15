using BookingSystem.Application.Auth.Responses;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;

