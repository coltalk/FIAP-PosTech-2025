using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using UsersAPI.Domain.Interfaces.Events;
using UsersAPI.Messaging;

namespace UsersAPI.IoC;

[ExcludeFromCodeCoverage]
public static class RabbitMQExtensions
{
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar o inicializador
        services.AddSingleton<RabbitMQInitializer>();

        // Registrar o publisher como Singleton (mantém a conexão aberta)
        services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

        return services;
    }
}