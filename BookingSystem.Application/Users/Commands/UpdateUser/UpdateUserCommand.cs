using MediatR;

namespace BookingSystem.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string Email
) : IRequest<Guid>;
