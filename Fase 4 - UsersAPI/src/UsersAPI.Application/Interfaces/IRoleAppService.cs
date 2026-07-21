using UsersAPI.Domain.Dtos.Request.Role;
using UsersAPI.Domain.Dtos.Responses.Role;

namespace UsersAPI.Application.Interfaces;

public interface IRoleAppService
{
    Task<RolesResponse> Cadastrar(CadastrarRoleRequest request);
    Task<List<RolesResponse>> ListarRoles();
    Task<(RolesResponse? Role, bool Success)> AtualizarRole(AtualizarRoleRequest request);
}
