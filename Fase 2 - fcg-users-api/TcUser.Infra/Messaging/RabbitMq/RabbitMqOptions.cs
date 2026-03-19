namespace TcUser.Infra.Messaging.RabbitMq;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = null!;
    public int Port { get; init; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; init; } = null!;


    public string UserCreatedQueue { get; set; } = null!;
}
