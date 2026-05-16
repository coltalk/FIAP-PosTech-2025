namespace UsersAPI.Domain.Dtos.Request.Role;

public record AtualizarRoleRequest
{
    public int Id { get; init; }
    public string RoleName { get; init; }
    public string? Description { get; init; }
}
