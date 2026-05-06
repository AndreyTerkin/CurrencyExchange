using FinanceService.Domain.Entities;
using FinanceService.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies => Set<Currency>();
    internal DbSet<UserFavoriteCurrencyEntity> UserFavoriteCurrencies => Set<UserFavoriteCurrencyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CurrencyEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserFavoriteCurrencyEntityConfiguration());
    }
}
