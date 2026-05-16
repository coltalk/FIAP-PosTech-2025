using UsersAPI.Domain.Enums;

namespace UsersAPI.Domain.Dtos.Request.Usuario;

public class CadastrarUsuarioRequest
{
    public string Nome { get; set; }
    public string Senha { get; set; }
    public string Celular { get; set; }
    public string Email { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public string NomeCompleto { get; set; }
    public DateTimeOffset? DataNascimento { get; set; }
    public string Pais { get; set; }
    public string AvatarUrl { get; set; }
}
