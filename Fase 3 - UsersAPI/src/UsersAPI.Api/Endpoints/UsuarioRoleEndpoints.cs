using UsersAPI.Api.Filters;
using UsersAPI.Api.Helpers;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.UsuarioRole;
using UsersAPI.Domain.Dtos.Responses.UsuarioRole;

namespace UsersAPI.Api.Endpoints;

public static class UsuarioRoleEndpoints
{
    public static void MapUsuarioRole(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/UsuarioRole").WithTags("UsuarioRole");

        app.MapGet("ListarRolesPorUsuario/", async (Guid usuarioId, IUsuarioRoleAppService usuarioService) =>
        {
            var request = new ListarRolePorUsuarioRequest(usuarioId);
            var result = await usuarioService.ListarRolesPorUsuario(request);
            if (result == null || !result.Any())
            {
                return ApiResponses.NotFound("roles", "Nenhuma role encontrada para este usuário.");
            }
            return ApiResponses.Ok(result, "Roles listadas com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<ListarRolePorUsuarioRequest>>()
        .WithName("ListarRolesPorUsuario")
        .Produces<List<ListarRolesPorUsuarioResponse>>(200)
        .Produces(400)
        .Produces(404);


        app.MapPut("AlterarRoleUsuario", async (AlterarUsuarioRoleRequest request, IUsuarioRoleAppService usuarioRoleService) =>
        {
            var result = await usuarioRoleService.AlterarRoleUsuario(request);
            if (!result)
            {
                return ApiResponses.NotFound("usuarioRole", "Registro não encontrado ou não foi possível atualizar.");
            }
            return ApiResponses.OkMessage("Role do usuário alterada com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<AlterarUsuarioRoleRequest>>()
        .WithName("AlterarRoleUsuario")
        .Produces(200)
        .Produces(400)
        .Produces(404);
    }
}