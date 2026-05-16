namespace UsersAPI.Domain.Dtos.Responses.UsuarioPerfil;

public record BuscarUsuarioPerfilResponse
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }
    public string NomeCompleto { get; init; }
    public DateTimeOffset? DataNascimento { get; init; }
    public string Pais { get; init; }
    public string AvatarUrl { get; init; }
}
