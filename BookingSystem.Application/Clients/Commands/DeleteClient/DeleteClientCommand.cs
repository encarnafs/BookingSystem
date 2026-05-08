using MediatR;

namespace BookingSystem.Application.Clients.Commands.DeleteClient;

public record DeleteClientCommand(Guid ClientId) : IRequest<Unit>;
