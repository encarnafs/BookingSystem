using BookingSystem.Api.Requests.Users;
using BookingSystem.Api.Responses.Users;
using BookingSystem.Application.Users.Commands.CreateUser;
using BookingSystem.Application.Users.Commands.UpdateUser;
using BookingSystem.Application.Users.Dtos;

namespace BookingSystem.Api.Mappers;

public static class UserMapper
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
        => new(request.Username, request.Email, request.Password, request.Role);

    public static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid id)
        => new(id, request.Email);

    public static UserResponse ToResponse(this UserDto dto)
        => new()
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            Role = dto.Role
        };
}
