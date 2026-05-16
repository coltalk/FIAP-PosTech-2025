namespace UsersAPI.Domain.Dtos.Request.Role;

public class CadastrarRoleRequest
{
    public int Id { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
}
