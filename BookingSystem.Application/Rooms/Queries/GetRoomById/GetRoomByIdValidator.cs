using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Rooms.Queries.GetRoomById;


public class GetRoomByIdValidator : AbstractValidator<GetRoomByIdQuery>
{
    public GetRoomByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id de la sala es obligatorio.");
    }
}
