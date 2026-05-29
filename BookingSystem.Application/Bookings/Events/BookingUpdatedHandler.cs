using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingUpdatedHandler : INotificationHandler<BookingUpdatedNotification>
{
    private readonly ILogger<BookingUpdatedHandler> _logger;

    public BookingUpdatedHandler(
        ILogger<BookingUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Reserva {BookingId} actualizada. OldValues: {@OldValues}, NewValues: {@NewValues}",
            notification.BookingId,
            notification.OldValues,
            notification.NewValues);

        return Task.CompletedTask;
    }
}
