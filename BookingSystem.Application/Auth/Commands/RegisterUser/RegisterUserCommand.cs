using BookingSystem.Application.Auth.Responses;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Username, string Email, string Password) : IRequest<AuthResponse>;
