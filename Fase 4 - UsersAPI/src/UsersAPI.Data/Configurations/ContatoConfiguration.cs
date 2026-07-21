using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersApi.Domain.Entities;

namespace UsersAPI.Data.Configurations;
public class ContatoConfiguration : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.ToTable("Contato");
        builder.HasKey(primaryKey => primaryKey.Id);

        builder.Property(e => e.Id)
           .ValueGeneratedOnAdd();

        builder.Property(c => c.Celular)
            .IsRequired(true);

        builder.Property(c => c.Email)
            .IsRequired(true);

        builder.Property(c => c.UsuarioId)
               .IsRequired();

        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Contatos)
            .HasForeignKey(c => new { c.UsuarioId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
