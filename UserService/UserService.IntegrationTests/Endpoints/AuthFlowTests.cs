using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using UserService.IntegrationTests.Infrastructure;

namespace UserService.IntegrationTests.Endpoints;

public class AuthFlowTests(UserServiceWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task FullFlow_RegisterLoginLogout_TokenInvalidatedAfterLogout()
    {
        // Register
        var registerResponse = await Client.PostAsJsonAsync("/api/users/register",
            new { name = "alice", password = "secret123" });
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Login
        var loginResponse = await Client.PostAsJsonAsync("/api/users/login",
            new { name = "alice", password = "secret123" });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        string token = (await loginResponse.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("token").GetString()!;

        // Token works — logout succeeds
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var logoutResponse = await Client.PostAsync("/api/users/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Same token is now blacklisted
        var repeatResponse = await Client.PostAsync("/api/users/logout", null);
        repeatResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
