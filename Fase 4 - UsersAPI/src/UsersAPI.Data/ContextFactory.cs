using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UsersAPI.Data;

public class ContextFactory : IDesignTimeDbContextFactory<UsersApiDbContext>
{
    public UsersApiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersApiDbContext>();
        optionsBuilder.UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=MS_UsersAPI;Integrated security=true");

        return new UsersApiDbContext(optionsBuilder.Options);
    }
}