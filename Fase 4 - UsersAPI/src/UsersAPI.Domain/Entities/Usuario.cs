namespace UsersApi.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Senha { get; set; }
    public bool Ativo { get; set; }
    public DateTimeOffset? DataCriacao { get; set; }
    public DateTimeOffset? DataAtualizacao { get; set; }


    public virtual UsuarioPerfil Perfil { get; set; }
    public virtual ICollection<UsuarioRole> UsuarioRoles { get; set; }
    public virtual ICollection<Endereco> Enderecos { get; set; }
    public virtual ICollection<Contato> Contatos { get; set; }

    public static Usuario Criar(string nome, string senha, bool ativo = true)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Senha = senha,
            Ativo = ativo,
            DataCriacao = DateTime.UtcNow
        };
    }

    public void AlterarSenha(string senha)
    {
        Senha = senha;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AlterarStatus(bool ativo)
    {
        Ativo = ativo;
        DataAtualizacao = DateTime.UtcNow;
    }
}