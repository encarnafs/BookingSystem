using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingCancelledHandler : INotificationHandler<BookingCancelledNotification>
{
    private readonly ILogger<BookingCancelledHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingCancelledHandler(
        ILogger<BookingCancelledHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingCancelledNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Procesando cancelación de reserva {BookingId}", notification.BookingId);

        _logger.LogInformation("Iniciando envío de email por cancelación de reserva {BookingId}", notification.BookingId);

        await _emailService.SendAsync(
            to: "encarnifs@gmail.com",
            subject: "Reserva cancelada",
            body: $"La reserva con ID {notification.BookingId} ha sido cancelada.");

        _logger.LogInformation("Email enviado por cancelación de reserva {BookingId}", notification.BookingId);
    }
}
