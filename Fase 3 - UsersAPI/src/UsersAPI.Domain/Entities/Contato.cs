namespace UsersApi.Domain.Entities;
public class Contato
{
    public Contato(string celular, string email)
    {
        Id = Guid.NewGuid();
        Celular = celular;
        Email = email;
    }

    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Celular { get; set; }
    public string Email { get; set; }

    public virtual Usuario Usuario { get; set; }
}