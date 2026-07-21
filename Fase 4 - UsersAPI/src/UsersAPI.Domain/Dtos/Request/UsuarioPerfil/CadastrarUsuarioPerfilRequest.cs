namespace UsersAPI.Domain.Dtos.Request.UsuarioPerfil;

public record CadastrarUsuarioPerfilRequest
{
    public Guid UsuarioId { get; init; }
    public string NomeCompleto { get; init; }
    public DateTimeOffset? DataNascimento { get; init; }
    public string Pais { get; init; }
    public string AvatarUrl { get; init; }
}
