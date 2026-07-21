using UsersApi.Domain.Entities;
using UsersAPI.Domain.Dtos.Request.Usuario;
using UsersAPI.Domain.Dtos.Responses.Usuario;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Services;

public interface IUsuarioService : IGenericServices<Usuario>
{
    Task<Usuario> CadastrarUsuario(CadastrarUsuarioRequest request);

    Task<Usuario> ValidarLogin(string usuario, string senha);

    Task<bool> AlterarSenha(AlterarSenhaRequest request);

    Task<AlterarStatusResponse> AlterarStatus(Guid Id);
}
