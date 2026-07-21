namespace UsersAPI.Domain.Dtos.Request.Contato;

public record AtualizarContatoRequest
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }
    public string Celular { get; init; }
    public string Email { get; init; }
}
