using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using UsersAPI.Application.AppServices;
using UsersAPI.Application.Interfaces;

namespace UsersAPI.IoC;

[ExcludeFromCodeCoverage]
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IContatoAppService, ContatoAppService>();
        services.AddScoped<IEnderecoAppService, EnderecoAppService>();
        services.AddScoped<IRoleAppService, RoleAppService>();
        services.AddScoped<IUsuarioAppService, UsuarioAppService>();
        services.AddScoped<IUsuarioPerfilAppService, UsuarioPerfilAppService>();
        services.AddScoped<IUsuarioRoleAppService, UsuarioRoleAppService>();
        return services;
    }
}
