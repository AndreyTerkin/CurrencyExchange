using FinanceService.Application.Queries.GetUserFavoriteCurrencies;
using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FinanceService.Tests.Queries;

public class GetUserFavoriteCurrenciesQueryHandlerTests
{
    private readonly Mock<ICurrencyRepository> _repoMock = new();
    private readonly GetUserFavoriteCurrenciesQueryHandler _handler;

    public GetUserFavoriteCurrenciesQueryHandlerTests()
    {
        _handler = new GetUserFavoriteCurrenciesQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_WithFavorites_ReturnsCurrencyList()
    {
        // Arrange
        var currencies = new List<Currency>
        {
            Currency.Create(1, "USD", "US Dollar", 92.50m),
            Currency.Create(2, "EUR", "Euro", 100.25m),
        };

        _repoMock.Setup(r => r.GetFavoritesByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        var query = new GetUserFavoriteCurrenciesQuery(1);

        // Act
        IReadOnlyList<CurrencyDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Code.Should().Be("USD");
        result[0].Name.Should().Be("US Dollar");
        result[0].Rate.Should().Be(92.50m);
        result[1].Code.Should().Be("EUR");
        result[1].Name.Should().Be("Euro");
        result[1].Rate.Should().Be(100.25m);
    }

    [Fact]
    public async Task Handle_WithNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        _repoMock.Setup(r => r.GetFavoritesByUserIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Currency>());

        var query = new GetUserFavoriteCurrenciesQuery(42);

        // Act
        IReadOnlyList<CurrencyDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
