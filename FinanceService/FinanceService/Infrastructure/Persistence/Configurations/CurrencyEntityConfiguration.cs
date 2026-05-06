using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistence.Configurations;

public class CurrencyEntityConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currency");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(c => c.Code).HasColumnName("code").HasMaxLength(10).IsRequired();
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Rate).HasColumnName("rate").HasColumnType("decimal(19,6)").IsRequired();
    }
}
