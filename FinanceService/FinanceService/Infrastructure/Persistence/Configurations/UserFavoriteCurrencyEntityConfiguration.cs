using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistence.Configurations;

internal class UserFavoriteCurrencyEntityConfiguration : IEntityTypeConfiguration<UserFavoriteCurrencyEntity>
{
    public void Configure(EntityTypeBuilder<UserFavoriteCurrencyEntity> builder)
    {
        builder.ToTable("user_favorite_currencies");
        builder.HasKey(ufc => new { ufc.UserId, ufc.CurrencyId });
        builder.Property(ufc => ufc.UserId).HasColumnName("user_id");
        builder.Property(ufc => ufc.CurrencyId).HasColumnName("currency_id");

        builder.HasOne(ufc => ufc.Currency)
            .WithMany()
            .HasForeignKey(ufc => ufc.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
