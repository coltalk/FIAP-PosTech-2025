namespace UsersApi.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<UsuarioRole> Usuarios { get; set; }
}