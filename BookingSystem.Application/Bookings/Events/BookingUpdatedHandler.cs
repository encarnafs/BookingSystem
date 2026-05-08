using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingUpdatedHandler : INotificationHandler<BookingUpdatedNotification>
{
    private readonly ILogger<BookingUpdatedHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingUpdatedHandler(
        ILogger<BookingUpdatedHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Procesando actualización completa de reserva {BookingId}", notification.BookingId);

        _logger.LogInformation("Iniciando envío de email por actualización de reserva {BookingId}", notification.BookingId);

        await _emailService.SendAsync(
            to: "encarnifs@gmail.com",
            subject: "Reserva actualizada",
            body: $"La reserva con ID {notification.BookingId} ha sido actualizada.");

        _logger.LogInformation("Email enviado por actualización de reserva {BookingId}", notification.BookingId);
    }
}
