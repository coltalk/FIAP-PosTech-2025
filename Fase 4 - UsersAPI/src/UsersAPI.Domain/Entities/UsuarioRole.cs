namespace UsersApi.Domain.Entities;

public class UsuarioRole
{

    public UsuarioRole() { }

    public UsuarioRole(int roleId)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
    }


    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    public int RoleId { get; set; }


    public virtual Usuario Usuario { get; set; }
    public virtual Role Role { get; set; }
}