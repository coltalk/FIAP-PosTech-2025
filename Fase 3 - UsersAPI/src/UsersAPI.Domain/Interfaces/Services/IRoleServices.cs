using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;

namespace UsersAPI.Domain.Interfaces.Services;

public interface IRoleServices : IGenericServices<Role>
{
    List<Role> ListarRoles();
    Task<(Role? Role, bool Success)> AtualizarRole(Role role);
}
