using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using UsersAPI.Domain.Interfaces.Services;
using UsersAPI.Domain.Services;

namespace UsersAPI.IoC;

[ExcludeFromCodeCoverage]
public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IContatoService, ContatoService>();
        services.AddScoped<IEnderecoService, EnderecoService>();
        services.AddScoped<IRoleServices, RoleServices>();
        services.AddScoped<IUsuarioService, UsuarioServices>();
        services.AddScoped<IUsuarioPerfilService, UsuarioPerfilServices>();
        services.AddScoped<IUsuarioRoleServices, UsuarioRoleServices>();
        return services;
    }
}
