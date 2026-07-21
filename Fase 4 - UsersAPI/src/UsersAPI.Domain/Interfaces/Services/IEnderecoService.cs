using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Services;

public interface IEnderecoService : IGenericServices<Endereco>
{
    List<Endereco> ListarPorUsuario(Guid usuarioId);
    Task<Endereco> Cadastrar(Endereco endereco);
    Task<(Endereco? Endereco, bool Success)> Atualizar(Endereco endereco);
    Task<bool> Deletar(Guid id, Guid usuarioId);
}
