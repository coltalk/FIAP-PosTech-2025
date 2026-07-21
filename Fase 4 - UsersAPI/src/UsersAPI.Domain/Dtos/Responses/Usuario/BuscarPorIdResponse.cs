namespace UsersAPI.Domain.Dtos.Responses.Usuario;

public record BuscarPorIdResponse
{
    public Guid Id { get; init; }
    public string? Nome { get; init; }
    public bool Ativo { get; init; }
    public DateTimeOffset? DataCriacao { get; init; }
    public DateTimeOffset? DataAtualizacao { get; init; }
    public UsuarioPerfilResponse? Perfil { get; init; }
    public List<UsuarioRoleResponse>? Roles { get; init; }
}

public record UsuarioPerfilResponse
{
    public Guid Id { get; init; }
    public string? NomeCompleto { get; init; }
    public DateTimeOffset? DataNascimento { get; init; }
    public string? Pais { get; init; }
    public string? AvatarUrl { get; init; }
}

public record UsuarioRoleResponse
{
    public Guid Id { get; init; }
    public int RoleId { get; init; }
    public string? RoleName { get; init; }
    public string? Description { get; init; }
}
