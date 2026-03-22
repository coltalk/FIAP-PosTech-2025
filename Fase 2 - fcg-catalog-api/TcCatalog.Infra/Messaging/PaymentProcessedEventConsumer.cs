using System.Text;
using System.Text.Json;
using TcCatalog.Application.Events;
using TcCatalog.Application.Interfaces;
using TcCatalog.Application.Interfaces.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TcCatalog.Infra.Messaging;

public class PaymentProcessedEventConsumer : BackgroundService, IPaymentProcessedEventConsumer
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentProcessedEventConsumer> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;

    private IConnection? _connection;
    private IModel? _channel;

    public PaymentProcessedEventConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentProcessedEventConsumer> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
    }

    public Task ConsumeAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
        => ProcessMessageAsync(paymentProcessedEvent, ct);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return ExecuteWithRetryAsync(stoppingToken);
    }

    private async Task ExecuteWithRetryAsync(CancellationToken stoppingToken)
    {
        var attempt = 0;

        if (string.IsNullOrWhiteSpace(_rabbitMqOptions.HostName))
        {
            _logger.LogWarning("RabbitMq__HostName não configurado. Consumidor de eventos será desativado.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                attempt++;
                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMqOptions.HostName,
                    Port = _rabbitMqOptions.Port,
                    UserName = _rabbitMqOptions.UserName,
                    Password = _rabbitMqOptions.Password,
                    VirtualHost = _rabbitMqOptions.VirtualHost,
                    DispatchConsumersAsync = true
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: _rabbitMqOptions.PaymentProcessedExchange,
                    type: ExchangeType.Fanout,
                    durable: true,
                    arguments: null);

                _channel.QueueDeclare(
                    queue: _rabbitMqOptions.PaymentProcessedQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueBind(
                    queue: _rabbitMqOptions.PaymentProcessedQueue,
                    exchange: _rabbitMqOptions.PaymentProcessedExchange,
                    routingKey: string.Empty);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (_, ea) =>
                {
                    try
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);

                        if (evt is null)
                        {
                            _logger.LogWarning("Mensagem de PaymentProcessedEvent inválida: {Message}", message);
                            _channel.BasicNack(ea.DeliveryTag, false, false);
                            return;
                        }

                        await ProcessMessageAsync(evt, stoppingToken);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar PaymentProcessedEvent");
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _channel.BasicConsume(
                    queue: _rabbitMqOptions.PaymentProcessedQueue,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation(
                    "Consumidor RabbitMQ iniciado para fila {Queue} ligada ao exchange {Exchange} (host: {Host}).",
                    _rabbitMqOptions.PaymentProcessedQueue,
                    _rabbitMqOptions.PaymentProcessedExchange,
                    _rabbitMqOptions.HostName);

                attempt = 0;

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _channel?.Dispose();
                _connection?.Dispose();
                _channel = null;
                _connection = null;

                if (attempt <= 3 || attempt % 6 == 0)
                {
                    _logger.LogWarning(
                        ex,
                        "Falha ao conectar no RabbitMQ (tentativa {Attempt}) em {Host}:{Port}. Tentando novamente em 10 segundos.",
                        attempt,
                        _rabbitMqOptions.HostName,
                        _rabbitMqOptions.Port);
                }
                else
                {
                    _logger.LogWarning(
                        "Falha ao conectar no RabbitMQ (tentativa {Attempt}) em {Host}:{Port}. Tentando novamente em 10 segundos.",
                        attempt,
                        _rabbitMqOptions.HostName,
                        _rabbitMqOptions.Port);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private async Task ProcessMessageAsync(PaymentProcessedEvent paymentProcessedEvent, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var bibliotecaJogoService = scope.ServiceProvider.GetRequiredService<IBibliotecaJogoService>();

        await bibliotecaJogoService.ProcessPaymentProcessedEventAsync(paymentProcessedEvent, ct);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
