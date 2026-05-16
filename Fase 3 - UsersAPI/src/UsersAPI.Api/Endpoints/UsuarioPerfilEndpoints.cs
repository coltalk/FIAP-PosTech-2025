using UsersAPI.Api.Filters;
using UsersAPI.Api.Helpers;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.UsuarioPerfil;
using UsersAPI.Domain.Dtos.Responses.UsuarioPerfil;

namespace UsersAPI.Api.Endpoints;

public static class UsuarioPerfilEndpoints
{
    public static void MapUsuarioPerfil(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/usuarios/{usuarioId:guid}/perfil").WithTags("UsuarioPerfil");

        app.MapGet("BuscarPorUsuarioId/", async (Guid usuarioId, IUsuarioPerfilAppService perfilService) =>
        {
            var perfil = await perfilService.BuscarPorUsuarioId(usuarioId);
            if (perfil == null)
            {
                return ApiResponses.NotFound("perfil", "Perfil não encontrado para este usuário.");
            }
            return ApiResponses.Ok(perfil, "Perfil encontrado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .WithName("BuscarPerfilDoUsuario")
        .Produces<BuscarUsuarioPerfilResponse>(200)
        .Produces(404);


        app.MapPost("Cadastrar/", async (Guid usuarioId, CadastrarUsuarioPerfilRequest request, IUsuarioPerfilAppService perfilService) =>
        {
            request = request with { UsuarioId = usuarioId };
            var perfil = await perfilService.Cadastrar(request);

            if (perfil == null)
            {
                return ApiResponses.Problem("Erro ao cadastrar o perfil.");
            }
            return ApiResponses.Created($"/api/usuarios/{usuarioId}/perfil/{perfil.Id}", perfil, "Perfil cadastrado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<CadastrarUsuarioPerfilRequest>>()
        .WithName("CadastrarPerfil")
        .Produces<BuscarUsuarioPerfilResponse>(201)
        .Produces(400);


        app.MapPut("Atualizar/{id:guid}", async (Guid usuarioId, Guid id, AtualizarUsuarioPerfilRequest request, IUsuarioPerfilAppService perfilService) =>
        {
            if (id != request.Id)
            {
                return ApiResponses.BadRequest("id", "Id da URL não corresponde ao Id do corpo da requisição.");
            }
            request = request with { UsuarioId = usuarioId };
            var (perfil, sucesso) = await perfilService.Atualizar(request);
            if (!sucesso || perfil == null)
            {
                return ApiResponses.NotFound("perfil", "Perfil não encontrado ou não pertence ao usuário.");
            }
            return ApiResponses.Ok(perfil, "Perfil atualizado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<AtualizarUsuarioPerfilRequest>>()
        .WithName("AtualizarPerfil")
        .Produces<BuscarUsuarioPerfilResponse>(200)
        .Produces(400)
        .Produces(404);


        app.MapDelete("Deletar/{id:guid}", async (Guid usuarioId, Guid id, IUsuarioPerfilAppService perfilService) =>
        {
            var sucesso = await perfilService.Deletar(id, usuarioId);
            if (!sucesso)
            {
                return ApiResponses.NotFound("perfil", "Perfil não encontrado ou não pertence ao usuário.");
            }
            return ApiResponses.OkMessage("Perfil removido com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .WithName("DeletarPerfil")
        .Produces(200)
        .Produces(404);
    }
}