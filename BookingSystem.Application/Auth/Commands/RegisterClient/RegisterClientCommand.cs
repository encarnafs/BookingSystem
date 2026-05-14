using BookingSystem.Application.Auth.Responses;
using MediatR;

namespace BookingSystem.Application.Auth.Commands.RegisterClient;

public record RegisterClientCommand(string FullName, string Email, string PhoneNumber, string Password) : IRequest<AuthResponse>;
