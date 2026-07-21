using Microsoft.EntityFrameworkCore;
using UsersApi.Domain.Entities;
using UsersAPI.Data.Configurations;

namespace UsersAPI.Data;

public class UsersApiDbContext : DbContext
{
    public UsersApiDbContext(DbContextOptions<UsersApiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; } = null!;
    public DbSet<Contato> Contatos { get; set; } = null!;
    public DbSet<Endereco> Enderecos { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UsuarioRole> UsuarioRoles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioPerfilConfiguration());
        modelBuilder.ApplyConfiguration(new ContatoConfiguration());
        modelBuilder.ApplyConfiguration(new EnderecoConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioRoleConfiguration());

        modelBuilder.Entity<Role>().HasData(
        new Role
        {
            Id = 1,
            RoleName = "usuario",
            Description = "Usuário padrão do sistema"
        },
        new Role
        {
            Id = 2,
            RoleName = "administrador",
            Description = "Administrador com acesso total"
        }
    );

        base.OnModelCreating(modelBuilder);
    }
}