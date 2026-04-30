using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DbMigrator.Data;

public class MigratorDbContextFactory : IDesignTimeDbContextFactory<MigratorDbContext>
{
    public MigratorDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException("Environment variable 'ConnectionStrings__DefaultConnection' is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<MigratorDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new MigratorDbContext(optionsBuilder.Options);
    }
}
