using Microsoft.EntityFrameworkCore;
using TcUser.Domain.Entities;

namespace TcUser.Infra.Data.Context.Sql.Context;

public class LojaGamesDbContext : DbContext
{
    public LojaGamesDbContext(DbContextOptions<LojaGamesDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LojaGamesDbContext).Assembly);
    }
}
