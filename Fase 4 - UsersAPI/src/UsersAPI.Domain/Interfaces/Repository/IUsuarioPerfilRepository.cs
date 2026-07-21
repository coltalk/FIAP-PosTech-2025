using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Repository;

public interface IUsuarioPerfilRepository : IGenericEntityRepository<UsuarioPerfil>
{
    UsuarioPerfil? BuscarPorUsuarioId(Guid usuarioId);
    UsuarioPerfil? BuscarPorIdEUsuario(Guid id, Guid usuarioId);
}
