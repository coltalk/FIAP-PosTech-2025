using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TcUser.Application.Events;
using TcUser.Application.Interfaces;

namespace TcUser.Infra.Messaging.RabbitMq;

public sealed class UserCreatedEventPublisher : IUserCreatedEventPublisher
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<UserCreatedEventPublisher> _logger;

    public UserCreatedEventPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<UserCreatedEventPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishAsync(UserCreatedEvent message, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _options.UserCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = nameof(UserCreatedEvent);
        properties.MessageId = Guid.NewGuid().ToString("N");
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _options.UserCreatedQueue,
            basicProperties: properties,
            body: payload);

        _logger.LogInformation(
            "Evento {EventName} publicado na fila {QueueName} para o usuário {UserId}",
            nameof(UserCreatedEvent),
            _options.UserCreatedQueue,
            message.UserId);

        return Task.CompletedTask;
    }
}
