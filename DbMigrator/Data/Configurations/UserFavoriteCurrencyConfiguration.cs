using DbMigrator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DbMigrator.Data.Configurations;

public class UserFavoriteCurrencyConfiguration : IEntityTypeConfiguration<UserFavoriteCurrency>
{
    public void Configure(EntityTypeBuilder<UserFavoriteCurrency> builder)
    {
        builder.ToTable("user_favorite_currencies");
        builder.HasKey(ufc => new { ufc.UserId, ufc.CurrencyId });
        builder.Property(ufc => ufc.UserId).HasColumnName("user_id");
        builder.Property(ufc => ufc.CurrencyId).HasColumnName("currency_id");

        builder.HasOne(ufc => ufc.User)
            .WithMany(u => u.FavoriteCurrencies)
            .HasForeignKey(ufc => ufc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ufc => ufc.Currency)
            .WithMany()
            .HasForeignKey(ufc => ufc.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
