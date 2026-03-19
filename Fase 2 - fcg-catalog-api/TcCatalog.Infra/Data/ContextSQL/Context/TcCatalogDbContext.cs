using Microsoft.EntityFrameworkCore;
using TcCatalog.Domain.Entities;

namespace TcCatalog.Infra.Data.Context.Sql.Context;

public class TcCatalogDbContext : DbContext
{
    public TcCatalogDbContext(DbContextOptions<TcCatalogDbContext> options)
        : base(options) { }

    public DbSet<Jogo> Jogos => Set<Jogo>();
    public DbSet<BibliotecaJogo> BibliotecaJogos => Set<BibliotecaJogo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TcCatalogDbContext).Assembly);
    }
}