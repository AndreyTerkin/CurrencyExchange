using DbMigrator.Data.Configurations;
using DbMigrator.Entities;
using Microsoft.EntityFrameworkCore;

namespace DbMigrator.Data;

public class MigratorDbContext(DbContextOptions<MigratorDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFavoriteCurrency> UserFavoriteCurrencies => Set<UserFavoriteCurrency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserFavoriteCurrencyConfiguration());
    }
}
