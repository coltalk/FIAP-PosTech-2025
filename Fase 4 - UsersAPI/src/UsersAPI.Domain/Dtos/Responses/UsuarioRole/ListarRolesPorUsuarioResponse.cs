namespace UsersAPI.Domain.Dtos.Responses.UsuarioRole;

public record ListarRolesPorUsuarioResponse
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }
    public int RoleId { get; init; }
    public string RoleName { get; init; }
    public string Description { get; init; }
}
