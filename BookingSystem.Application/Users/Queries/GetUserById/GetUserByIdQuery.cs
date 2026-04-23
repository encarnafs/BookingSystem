using BookingSystem.Application.Users.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

