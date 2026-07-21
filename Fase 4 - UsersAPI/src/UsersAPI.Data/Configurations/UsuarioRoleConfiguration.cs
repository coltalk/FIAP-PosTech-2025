using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersApi.Domain.Entities;

namespace UsersAPI.Data.Configurations;

public class UsuarioRoleConfiguration : IEntityTypeConfiguration<UsuarioRole>
{
    public void Configure(EntityTypeBuilder<UsuarioRole> builder)
    {
        builder.ToTable("UsuarioRole");

        builder.HasKey(primaryKey => primaryKey.Id);

        builder.Property(ur => ur.Id)
               .ValueGeneratedNever();

        builder.Property(c => c.UsuarioId)
               .IsRequired();

        builder.Property(ur => ur.RoleId)
               .IsRequired();

        // Usuario (1) -> UsuarioRole (N)
        builder.HasOne(ur => ur.Usuario)
               .WithMany(u => u.UsuarioRoles)
               .HasForeignKey(c => new { c.UsuarioId })
               .OnDelete(DeleteBehavior.Restrict);

        // Role (1) -> UsuarioRole (N)
        builder.HasOne(ur => ur.Role)
               .WithMany(r => r.Usuarios)
               .HasForeignKey(ur => ur.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ur => new { ur.UsuarioId, ur.RoleId }).IsUnique();
    }
}