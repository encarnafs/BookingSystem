using BookingSystem.Application.Auth.Responses;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

