using System.Text;
using System.Text.Json;
using TcCatalog.Application.Events;
using TcCatalog.Application.Interfaces.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace TcCatalog.Infra.Messaging;

public class OrderPlacedEventPublisher : IOrderPlacedEventPublisher
{
    private readonly ILogger<OrderPlacedEventPublisher> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;

    public OrderPlacedEventPublisher(
        ILogger<OrderPlacedEventPublisher> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
    }

    public Task PublishAsync(OrderPlacedEvent orderPlacedEvent, CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password,
            VirtualHost = _rabbitMqOptions.VirtualHost
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _rabbitMqOptions.OrderPlacedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var payload = JsonSerializer.Serialize(orderPlacedEvent);
        var body = Encoding.UTF8.GetBytes(payload);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _rabbitMqOptions.OrderPlacedQueue,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "OrderPlacedEvent publicado na fila {Queue}: {Payload}",
            _rabbitMqOptions.OrderPlacedQueue,
            payload);

        return Task.CompletedTask;
    }
}
