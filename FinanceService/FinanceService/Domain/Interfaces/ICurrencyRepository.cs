using FinanceService.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetFavoritesByUserIdAsync(int userId, CancellationToken ct);
}
