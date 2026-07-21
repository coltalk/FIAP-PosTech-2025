using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Repository;

public interface IContatoRepository : IGenericEntityRepository<Contato>
{
    List<Contato> ListarPorUsuario(Guid usuarioId);
    Contato? BuscarPorIdEUsuario(Guid id, Guid usuarioId);
}
