using BookingSystem.Application.Clients.Commands.CreateClient;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Clients.Queries.GetClientById;

public class GetClientByIdValidator: AbstractValidator<GetClientByIdQuery>
{
        public GetClientByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id del cliente es obligatorio");
    }
}
