using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingDatesUpdatedHandler : INotificationHandler<BookingDatesUpdatedNotification>
{
    private readonly ILogger<BookingDatesUpdatedHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingDatesUpdatedHandler(
        ILogger<BookingDatesUpdatedHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingDatesUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Procesando actualización de fechas de reserva {BookingId}", notification.BookingId);

        await _emailService.SendAsync(
            to: notification.Email,
            subject: "Fechas de reserva actualizadas",
            body: $"Las fechas de tu reserva con ID {notification.BookingId} han sido actualizadas correctamente.");

        _logger.LogInformation("Email enviado por actualización de fechas {BookingId}", notification.BookingId);
    }
}
