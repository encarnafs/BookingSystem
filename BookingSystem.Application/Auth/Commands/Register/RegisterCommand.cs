using BookingSystem.Application.Auth.Responses;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.Register;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<AuthResponse>;
