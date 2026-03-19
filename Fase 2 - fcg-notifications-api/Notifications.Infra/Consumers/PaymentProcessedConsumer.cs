using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Application.Events;
using Notifications.Application.Interfaces;

namespace Notifications.Infra.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(
        INotificationService notificationService,
        ILogger<PaymentProcessedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        _logger.LogInformation(
            "Evento PaymentProcessedEvent recebido via RabbitMQ | UserId: {UserId} | Status: {Status}",
            context.Message.UserId, context.Message.Status);

        await _notificationService.HandlePaymentProcessedAsync(context.Message, context.CancellationToken);
    }
}
