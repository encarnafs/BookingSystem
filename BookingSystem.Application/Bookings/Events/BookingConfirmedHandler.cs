using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingConfirmedHandler : INotificationHandler<BookingConfirmedNotification>
{
    private readonly ILogger<BookingConfirmedHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingConfirmedHandler(
        ILogger<BookingConfirmedHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingConfirmedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Procesando confirmación de reserva {BookingId}", notification.BookingId);

        _logger.LogInformation("Iniciando envío de email por confirmación de reserva {BookingId}", notification.BookingId);

        await _emailService.SendAsync(
            to: "admin@bookingsystem.com",
            subject: "Reserva confirmada",
            body: $"La reserva con ID {notification.BookingId} ha sido confirmada.");

        _logger.LogInformation("Email enviado por confirmación de reserva {BookingId}", notification.BookingId);
    }
}
