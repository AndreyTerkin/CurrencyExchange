using MediatR;

namespace FinanceService.Application.Queries.GetUserFavoriteCurrencies;

public record GetUserFavoriteCurrenciesQuery(int UserId) : IRequest<IReadOnlyList<CurrencyDto>>;
