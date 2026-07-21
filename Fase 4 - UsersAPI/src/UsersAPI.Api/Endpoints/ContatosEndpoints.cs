using UsersAPI.Api.Filters;
using UsersAPI.Api.Helpers;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Contato;
using UsersAPI.Domain.Dtos.Responses.Contato;

namespace UsersAPI.Api.Endpoints;
public static class ContatoEndpoints
{
    public static void MapContatos(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/usuarios/{usuarioId:guid}/contatos").WithTags("Contatos");

        app.MapGet("BuscarPorUsuarioId/", async (Guid usuarioId, IContatoAppService contatoService) =>
        {
            var contatos = await contatoService.ListarPorUsuario(usuarioId);
            return ApiResponses.Ok(contatos, "Contatos listados com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .WithName("ListarContatosDoUsuario")
        .Produces<List<ContatoResponse>>(200);


        app.MapPost("Cadastrar/", async (Guid usuarioId, CadastrarContatoRequest request, IContatoAppService contatoService) =>
        {
            request = request with { UsuarioId = usuarioId };
            var contato = await contatoService.Cadastrar(request);
            return ApiResponses.Created($"/api/usuarios/{usuarioId}/contatos/{contato.Id}", contato, "Contato cadastrado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<CadastrarContatoRequest>>()
        .WithName("CadastrarContato")
        .Produces<ContatoResponse>(201)
        .Produces(400);

        app.MapPut("Atualizar/{id:guid}", async (Guid usuarioId, Guid id, AtualizarContatoRequest request, IContatoAppService contatoService) =>
        {
            if (id != request.Id)
            {
                return ApiResponses.BadRequest("id", "Id da URL não corresponde ao Id do corpo da requisição.");
            }

            request = request with { UsuarioId = usuarioId };
            var (contato, sucesso) = await contatoService.Atualizar(request);

            if (!sucesso || contato == null)
            {
                return ApiResponses.NotFound("contato", "Contato não encontrado ou não pertence ao usuário.");
            }

            return ApiResponses.Ok(contato, "Contato atualizado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<AtualizarContatoRequest>>()
        .WithName("AtualizarContato")
        .Produces<ContatoResponse>(200)
        .Produces(400)
        .Produces(404);


        app.MapDelete("Deletar/{id:guid}", async (Guid usuarioId, Guid id, IContatoAppService contatoService) =>
        {
            var sucesso = await contatoService.Deletar(id, usuarioId);

            if (!sucesso)
            {
                return ApiResponses.NotFound("contato", "Contato não encontrado ou não pertence ao usuário.");
            }

            return ApiResponses.OkMessage("Contato removido com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .WithName("DeletarContato")
        .Produces(200)
        .Produces(404);
    }
}