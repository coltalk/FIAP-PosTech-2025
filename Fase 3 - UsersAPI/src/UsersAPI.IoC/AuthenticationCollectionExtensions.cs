using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using UsersAPI.Application.AppServices;
using UsersAPI.Application.Interfaces;
using UsersAPI.Authentication;

namespace UsersAPI.IoC;

[ExcludeFromCodeCoverage]
public static class AuthenticationCollectionExtensions
{
    public static void AddAuthenticationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();
    }
}
