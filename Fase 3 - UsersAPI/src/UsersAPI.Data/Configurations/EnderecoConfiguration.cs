using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersApi.Domain.Entities;

namespace UsersAPI.Data.Configurations;
public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Endereco");
        builder.HasKey(primaryKey => primaryKey.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Rua)
              .IsRequired(true)
              .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Numero)
               .IsRequired(true)
               .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Complemento)
               .IsRequired(false)
               .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Bairro)
               .IsRequired(true)
               .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Cidade)
               .IsRequired(true)
               .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Estado)
               .IsRequired(true)
               .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Cep)
               .IsRequired(true)
               .HasColumnType("nvarchar(max)");

        builder.Property(c => c.UsuarioId)
               .IsRequired();

        builder.HasOne(e => e.Usuario)
              .WithMany(u => u.Enderecos)
              .HasForeignKey(c => new { c.UsuarioId })
              .OnDelete(DeleteBehavior.Cascade);
    }
}