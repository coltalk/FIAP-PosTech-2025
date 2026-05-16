using UsersAPI.Domain.Enums;

namespace UsersAPI.Domain.Dtos.Request.UsuarioRole;

public record AlterarUsuarioRoleRequest
{
    public Guid IdUsuarioRole { get; init; }
    public Guid UsuarioId { get; init; }
    public TipoUsuario TipoUsuario { get; init; }
}
