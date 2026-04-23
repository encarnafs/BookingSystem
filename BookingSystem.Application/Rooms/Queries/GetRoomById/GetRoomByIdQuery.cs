using BookingSystem.Application.Rooms.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Rooms.Queries.GetRoomById;


public record GetRoomByIdQuery(Guid Id) : IRequest<RoomDto>;
