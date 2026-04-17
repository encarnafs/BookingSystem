
using BookingSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingSystem.Api.Authorization;

public class CanCancelBookingHandler
    : AuthorizationHandler<CanCancelBookingRequirement, Guid>
{
    private readonly IBookingRepository _bookingRepository;

    public CanCancelBookingHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanCancelBookingRequirement requirement,
        Guid bookingId)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return;

        var role = context.User.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
        {
            context.Succeed(requirement);
            return;
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken: default);
        if (booking is null)
            return;

        if (booking.ClientId.ToString() == userId)
        {
            context.Succeed(requirement);
        }
    }
}
