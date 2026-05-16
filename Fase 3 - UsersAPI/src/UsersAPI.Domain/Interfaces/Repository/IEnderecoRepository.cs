using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Repository;

public interface IEnderecoRepository : IGenericEntityRepository<Endereco>
{
    List<Endereco> ListarPorUsuario(Guid usuarioId);
    Endereco? BuscarPorIdEUsuario(Guid id, Guid usuarioId);
}
