namespace FinanceService.Domain.Entities;

public class UserFavoriteCurrency
{
    public int UserId { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
}
