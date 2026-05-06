using FinanceService.Domain.Entities;

namespace FinanceService.Infrastructure.Persistence;

internal class UserFavoriteCurrencyEntity
{
    public int UserId { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
}
