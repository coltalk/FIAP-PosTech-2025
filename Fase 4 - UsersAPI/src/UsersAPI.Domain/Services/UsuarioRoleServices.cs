using UsersApi.Domain.Entities;
using UsersAPI.Domain.Interfaces.Generic;
using UsersAPI.Domain.Interfaces.Services;
using UsersAPI.Domain.Services.Generic;

namespace UsersAPI.Domain.Services;

public class UsuarioRoleServices : GenericServices<UsuarioRole>, IUsuarioRoleServices
{
    public UsuarioRoleServices(IGenericEntityRepository<UsuarioRole> repository) : base(repository)
    {
    }
}
