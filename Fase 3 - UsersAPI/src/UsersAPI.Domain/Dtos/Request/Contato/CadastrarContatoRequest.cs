namespace UsersAPI.Domain.Dtos.Request.Contato;

public record CadastrarContatoRequest
{
    public Guid UsuarioId { get; init; }
    public string Celular { get; init; }
    public string Email { get; init; }
}
