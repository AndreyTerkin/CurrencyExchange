using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using UserService.IntegrationTests.Infrastructure;

namespace UserService.IntegrationTests.Endpoints;

public class LoginEndpointTests(UserServiceWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private const string Url = "/api/users/login";

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        await RegisterAsync("alice", "secret123");

        var response = await Client.PostAsJsonAsync(Url, new { name = "alice", password = "secret123" });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ValidCredentials_TokenContainsCorrectClaims()
    {
        int userId = await RegisterAsync("alice", "secret123");
        string token = await LoginAsync("alice", "secret123");

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Subject.Should().Be(userId.ToString());
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value.Should().Be("alice");
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns400()
    {
        await RegisterAsync("alice", "secret123");

        var response = await Client.PostAsJsonAsync(Url, new { name = "alice", password = "wrongpassword" });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.GetProperty("detail").GetString().Should().Be("Invalid credentials.");
    }

    [Fact]
    public async Task Login_UnknownUser_Returns400()
    {
        var response = await Client.PostAsJsonAsync(Url, new { name = "nonexistent", password = "secret123" });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.GetProperty("detail").GetString().Should().Be("Invalid credentials.");
    }

    [Fact]
    public async Task Login_EmptyCredentials_Returns400()
    {
        var response = await Client.PostAsJsonAsync(Url, new { name = "", password = "" });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("must not be empty");
    }

    private async Task<int> RegisterAsync(string name, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/users/register", new { name, password });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetInt32();
    }

    private async Task<string> LoginAsync(string name, string password)
    {
        var response = await Client.PostAsJsonAsync(Url, new { name, password });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }
}
