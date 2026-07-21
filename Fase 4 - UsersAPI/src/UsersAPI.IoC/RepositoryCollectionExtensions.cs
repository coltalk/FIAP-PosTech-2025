using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using UsersAPI.Data.Repositories;
using UsersAPI.Data.Repositories.Generic;
using UsersAPI.Domain.Interfaces.Generic;
using UsersAPI.Domain.Interfaces.Repository;

namespace UsersAPI.IoC;

[ExcludeFromCodeCoverage]
public static class RepositoryCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericEntityRepository<>), typeof(GenericEntityRepository<>));
        services.AddScoped<IEnderecoRepository, EnderecoRepository>();
        services.AddScoped<IContatoRepository, ContatoRepository>();
        services.AddScoped<IUsuarioPerfilRepository, UsuarioPerfilRepository>();
        return services;
    }
}
