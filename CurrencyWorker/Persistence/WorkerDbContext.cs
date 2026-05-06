using Microsoft.EntityFrameworkCore;

namespace CurrencyWorker.Persistence;

public class WorkerDbContext(DbContextOptions<WorkerDbContext> options) : DbContext(options)
{
    public DbSet<CurrencyEntity> Currencies => Set<CurrencyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyEntity>(builder =>
        {
            builder.ToTable("currency");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            builder.Property(c => c.Code).HasColumnName("code").HasMaxLength(10).IsRequired();
            builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            builder.Property(c => c.Rate).HasColumnName("rate").HasColumnType("decimal(19,6)").IsRequired();
        });
    }
}
