using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Application.Events;
using Notifications.Application.Interfaces;

namespace Notifications.Infra.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(
        INotificationService notificationService,
        ILogger<UserCreatedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        _logger.LogInformation(
            "Evento UserCreatedEvent recebido via RabbitMQ | UserId: {UserId}",
            context.Message.UserId);

        await _notificationService.HandleUserCreatedAsync(context.Message, context.CancellationToken);
    }
}
