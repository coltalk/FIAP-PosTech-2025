using UsersAPI.Domain.Dtos.Request.Enderecos;
using UsersAPI.Domain.Dtos.Responses.Endereco;

namespace UsersAPI.Application.Interfaces;

public interface IEnderecoAppService
{
    Task<List<EnderecoResponse>> ListarPorUsuario(Guid usuarioId);
    Task<List<EnderecoResponse>> ListarPaginacao(int take, int skip);
    Task<EnderecoResponse> Cadastrar(CadastrarEnderecoRequest request);
    Task<(EnderecoResponse? Endereco, bool Success)> Atualizar(AtualizarEnderecoRequest request);
    Task<bool> Deletar(Guid id, Guid usuarioId);
}
