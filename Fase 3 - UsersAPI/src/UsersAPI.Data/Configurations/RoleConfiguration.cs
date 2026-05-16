using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersApi.Domain.Entities;

namespace UsersAPI.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.HasKey(primary => primary.Id);

        builder.Property(r => r.Id)
               .ValueGeneratedNever();

        builder.Property(r => r.RoleName)
               .IsRequired()
               .HasColumnType("nvarchar(max)");

        builder.Property(r => r.Description)
               .IsRequired(false)
               .HasColumnType("nvarchar(max)");

        builder.HasMany(r => r.Usuarios)
             .WithOne(ur => ur.Role)
             .HasForeignKey(ur => ur.RoleId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}