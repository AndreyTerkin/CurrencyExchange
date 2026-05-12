using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceService.IntegrationTests.Infrastructure;

namespace FinanceService.IntegrationTests.Endpoints;

public class GetFavoritesEndpointTests(FinanceServiceWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private const string Url = "/api/currencies/favorites";

    [Fact]
    public async Task GetFavorites_WithoutToken_Returns401()
    {
        var response = await Client.GetAsync(Url);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFavorites_WithInvalidToken_Returns401()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.valid.jwt");

        var response = await Client.GetAsync(Url);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFavorites_NoFavorites_ReturnsEmptyList()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken(userId: 1));

        var response = await Client.GetAsync(Url);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task GetFavorites_WithFavorites_ReturnsCurrencies()
    {
        int[] currencies = await SeedCurrenciesAsync(
            ("USD", "US Dollar", 89.50m),
            ("EUR", "Euro", 97.30m));
        await SeedFavoritesAsync(userId: 1, currencies);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken(userId: 1));

        var response = await Client.GetAsync(Url);
        var body = await response.Content.ReadFromJsonAsync<JsonElement[]>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().HaveCount(2);
        body.Should().Contain(e => e.GetProperty("code").GetString() == "USD"
                                && e.GetProperty("name").GetString() == "US Dollar"
                                && e.GetProperty("rate").GetDecimal() == 89.50m);
        body.Should().Contain(e => e.GetProperty("code").GetString() == "EUR"
                                && e.GetProperty("name").GetString() == "Euro"
                                && e.GetProperty("rate").GetDecimal() == 97.30m);
    }
}
