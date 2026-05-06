using FinanceService.Domain.Interfaces;
using MediatR;

namespace FinanceService.Application.Queries.GetUserFavoriteCurrencies;

public class GetUserFavoriteCurrenciesQueryHandler(ICurrencyRepository repository)
    : IRequestHandler<GetUserFavoriteCurrenciesQuery, IReadOnlyList<CurrencyDto>>
{
    public async Task<IReadOnlyList<CurrencyDto>> Handle(
        GetUserFavoriteCurrenciesQuery request,
        CancellationToken cancellationToken)
    {
        var currencies = await repository.GetFavoritesByUserIdAsync(request.UserId, cancellationToken);
        return currencies.Select(c => new CurrencyDto(c.Code, c.Name, c.Rate)).ToList();
    }
}
