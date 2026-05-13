using BookingSystem.Application.Bookings.Dtos;
using BookingSystem.Application.Clients.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Clients.Queries.GetClientById;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientDto>
{

}
