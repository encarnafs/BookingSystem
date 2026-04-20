using MediatR;
using Microsoft.Extensions.Logging;
using BookingSystem.Application.Common.Interfaces;

namespace BookingSystem.Application.Bookings.Events;

public class BookingCreatedHandler : INotificationHandler<BookingCreatedNotification>
{
    private readonly ILogger<BookingCreatedHandler> _logger;
    private readonly IEmailService _emailService;

    public BookingCreatedHandler(
        ILogger<BookingCreatedHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(BookingCreatedNotification notification, CancellationToken cancellationToken)
    {
        // 1. Log
        _logger.LogInformation("Reserva creada con ID: {BookingId}", notification.BookingId);

        // 2. Email falso
        await _emailService.SendAsync(
            to: "admin@bookingsystem.com",
            subject: "Nueva reserva creada",
            body: $"Se ha creado la reserva con ID {notification.BookingId}");
    }
}
