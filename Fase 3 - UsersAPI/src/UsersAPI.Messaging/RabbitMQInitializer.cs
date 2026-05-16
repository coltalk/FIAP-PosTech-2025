using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace UsersAPI.Messaging;

public class RabbitMQInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQInitializer> _logger;

    public RabbitMQInitializer(IConfiguration configuration, ILogger<RabbitMQInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "admin",
            Password = _configuration["RabbitMQ:Password"] ?? "admin",
            VirtualHost = "/",
            Port = 5672
        };

        try
        {
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // CONFIGURAÇÃO PARA PUBLICAR (UserCreated)
            var userCreatedExchange = _configuration["RabbitMQ:Exchanges:UserCreated"] ?? "user-created-exchange";

            await channel.ExchangeDeclareAsync(
                exchange: userCreatedExchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            var userCreatedQueue = "user-created-queue-notifications";
            await channel.QueueDeclareAsync(
                queue: userCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            await channel.QueueBindAsync(
                queue: userCreatedQueue,
                exchange: userCreatedExchange,
                routingKey: "",
                arguments: null
            );

            _logger.LogInformation(
                "RabbitMQ inicializado | Exchange: {Exchange} | Queue: {Queue}",
                userCreatedExchange,
                userCreatedQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar RabbitMQ");
            throw;
        }
    }
}