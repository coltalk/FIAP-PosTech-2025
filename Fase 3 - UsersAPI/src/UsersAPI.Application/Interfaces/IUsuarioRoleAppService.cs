using UsersAPI.Domain.Dtos.Request.UsuarioRole;
using UsersAPI.Domain.Dtos.Responses.UsuarioRole;

namespace UsersAPI.Application.Interfaces;

public interface IUsuarioRoleAppService
{
    Task<IEnumerable<ListarRolesPorUsuarioResponse>> ListarRolesPorUsuario(ListarRolePorUsuarioRequest request);
    Task<bool> AlterarRoleUsuario(AlterarUsuarioRoleRequest request);
}
