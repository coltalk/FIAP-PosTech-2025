namespace TcCatalog.Infra.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = null!;
    public int Port { get; init; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; init; } = null!;
	
    public string OrderPlacedQueue { get; init; } = null!;
    public string PaymentProcessedExchange { get; init; } = null!;
    public string PaymentProcessedQueue { get; init; } = null!;
}
