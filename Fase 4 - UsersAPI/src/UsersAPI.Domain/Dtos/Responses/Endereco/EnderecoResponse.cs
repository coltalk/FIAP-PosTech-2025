namespace UsersAPI.Domain.Dtos.Responses.Endereco;

public record EnderecoResponse
{
    public Guid Id { get; init; }
    public Guid UsuarioId { get; init; }
    public string Rua { get; init; }
    public string Numero { get; init; }
    public string? Complemento { get; init; }
    public string Bairro { get; init; }
    public string Cidade { get; init; }
    public string Estado { get; init; }
    public string Cep { get; init; }
}
