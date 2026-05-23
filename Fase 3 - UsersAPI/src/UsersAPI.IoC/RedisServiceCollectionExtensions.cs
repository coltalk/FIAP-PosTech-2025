using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using UsersAPI.Application.Interfaces;
using UsersAPI.Application.Services;

namespace UsersAPI.IoC;

[ExcludeFromCodeCoverage]
public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? "localhost:6379";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "UsersAPI:";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
