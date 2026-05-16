namespace UsersApi.Domain.Entities;

public class Endereco
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Rua { get; set; }
    public string Numero { get; set; }
    public string? Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }

    public virtual Usuario Usuario { get; set; }
}