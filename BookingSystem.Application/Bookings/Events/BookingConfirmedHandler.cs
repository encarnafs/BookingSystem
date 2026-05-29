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

        await _emailService.SendAsync(
            to: notification.Email,
            subject: "Reserva confirmada",
            body: $"Tu reserva con ID {notification.BookingId} ha sido confirmada correctamente.");

        _logger.LogInformation("Email enviado por confirmación de reserva {BookingId}", notification.BookingId);
    }
}
