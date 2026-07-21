using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Services;

public interface IUsuarioPerfilService : IGenericServices<UsuarioPerfil>
{
    UsuarioPerfil? BuscarPorUsuarioId(Guid usuarioId);
    Task<UsuarioPerfil> Cadastrar(UsuarioPerfil perfil);
    Task<(UsuarioPerfil? Perfil, bool Success)> Atualizar(UsuarioPerfil perfil);
    Task<bool> Deletar(Guid id, Guid usuarioId);
}
