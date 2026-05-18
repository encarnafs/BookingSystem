using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Users.Events;

public record UserEnabledNotification(Guid UserId) : INotification;