using BookingSystem.Application.Auth.Responses;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.LoginClient;

public sealed record LoginClientCommand(string Email, string Password)
    : IRequest<AuthResponse>;
