using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Entities;
using BookingSystem.Application.Users.Dtos;
using MediatR;

namespace BookingSystem.Application.Users.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public CreateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Hash temporal (lo cambiaremos más adelante)
        var fakeHash = $"HASHED_{request.Password}";

        var user = new User(
            request.Username,
            request.Email,
            fakeHash,
            request.Role
        );

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
        
    }
}
