using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using UserService.IntegrationTests.Infrastructure;

namespace UserService.IntegrationTests.Endpoints;

public class LogoutEndpointTests(UserServiceWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private const string Url = "/api/users/logout";

    [Fact]
    public async Task Logout_ValidToken_Returns204()
    {
        string token = await GetTokenAsync();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await Client.PostAsync(Url, null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_AfterLogout_SameTokenReturns401()
    {
        string token = await GetTokenAsync();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await Client.PostAsync(Url, null);
        var response = await Client.PostAsync(Url, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithoutToken_Returns401()
    {
        var response = await Client.PostAsync(Url, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithInvalidToken_Returns401()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.valid.jwt");

        var response = await Client.PostAsync(Url, null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<string> GetTokenAsync()
    {
        await Client.PostAsJsonAsync("/api/users/register", new { name = "alice", password = "secret123" });
        var response = await Client.PostAsJsonAsync("/api/users/login", new { name = "alice", password = "secret123" });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }
}
