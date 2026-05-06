using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public class CurrencyRepository(FinanceDbContext context) : ICurrencyRepository
{
    public async Task<IReadOnlyList<Currency>> GetFavoritesByUserIdAsync(int userId, CancellationToken ct)
    {
        return await context.UserFavoriteCurrencies
            .Where(ufc => ufc.UserId == userId)
            .Select(ufc => ufc.Currency)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
