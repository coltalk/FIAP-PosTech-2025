using UsersAPI.Api.Filters;
using UsersAPI.Api.Helpers;
using UsersAPI.Application.Interfaces;
using UsersAPI.Domain.Dtos.Request.Enderecos;
using UsersAPI.Domain.Dtos.Responses.Endereco;

namespace UsersAPI.Api.Endpoints;

public static class EnderecoEndpoints
{
    public static void MapEnderecos(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/usuarios/{usuarioId:guid}/enderecos").WithTags("Enderecos");

        app.MapGet("BuscarPorUsuarioId/", async (Guid usuarioId, IEnderecoAppService enderecoService) =>
        {
            var enderecos = await enderecoService.ListarPorUsuario(usuarioId);
            return ApiResponses.Ok(enderecos, "Endereços listados com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .WithName("ListarEnderecosDoUsuario")
        .Produces<List<EnderecoResponse>>(200);


        app.MapPost("Cadastrar/", async (Guid usuarioId, CadastrarEnderecoRequest request, IEnderecoAppService enderecoService) =>
        {
            request = request with { UsuarioId = usuarioId };
            var endereco = await enderecoService.Cadastrar(request);
            return ApiResponses.Created($"/api/usuarios/{usuarioId}/enderecos/{endereco.Id}", endereco, "Endereço cadastrado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<CadastrarEnderecoRequest>>()
        .WithName("CadastrarEndereco")
        .Produces<EnderecoResponse>(201)
        .Produces(400);


        app.MapPut("Atualizar/{id:guid}", async (Guid usuarioId, Guid id, AtualizarEnderecoRequest request, IEnderecoAppService enderecoService) =>
        {
            if (id != request.Id)
            {
                return ApiResponses.BadRequest("id", "Id da URL não corresponde ao Id do corpo da requisição.");
            }

            // Garante que pertence ao usuário correto
            request = request with { UsuarioId = usuarioId };
            var (endereco, sucesso) = await enderecoService.Atualizar(request);

            if (!sucesso || endereco == null)
            {
                return ApiResponses.NotFound("endereco", "Endereço não encontrado ou não pertence ao usuário.");
            }

            return ApiResponses.Ok(endereco, "Endereço atualizado com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .AddEndpointFilter<ValidationEndpointFilter<AtualizarEnderecoRequest>>()
        .WithName("AtualizarEndereco")
        .Produces<EnderecoResponse>(200)
        .Produces(400)
        .Produces(404);


        app.MapDelete("Deletar/{id:guid}", async (Guid usuarioId, Guid id, IEnderecoAppService enderecoService) =>
        {
            var sucesso = await enderecoService.Deletar(id, usuarioId);

            if (!sucesso)
            {
                return ApiResponses.NotFound("endereco", "Endereço não encontrado ou não pertence ao usuário.");
            }

            return ApiResponses.OkMessage("Endereço removido com sucesso.");
        })
        .RequireAuthorization(policy => policy.RequireRole("usuario"))
        .WithName("DeletarEndereco")
        .Produces(200)
        .Produces(404);
    }
}