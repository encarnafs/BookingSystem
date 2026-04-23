using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.Users.Dtos;
using MediatR;
using BookingSystem.Domain.Entities;
using BookingSystem.Application.Common.Exceptions;


namespace BookingSystem.Application.Users.Queries.GetUserById;


public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
            throw new NotFoundException($"Usuario con Id {request.Id} no encontrado.", request.Id);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}