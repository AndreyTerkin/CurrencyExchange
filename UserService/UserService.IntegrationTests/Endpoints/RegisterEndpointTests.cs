using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using UserService.IntegrationTests.Infrastructure;

namespace UserService.IntegrationTests.Endpoints;

public class RegisterEndpointTests(UserServiceWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private const string Url = "/api/users/register";

    [Fact]
    public async Task Register_ValidData_Returns200WithId()
    {
        var response = await Client.PostAsJsonAsync(Url, new { name = "alice", password = "secret123" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Register_DuplicateName_Returns400()
    {
        await Client.PostAsJsonAsync(Url, new { name = "alice", password = "secret123" });

        var response = await Client.PostAsJsonAsync(Url, new { name = "alice", password = "another123" });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.GetProperty("detail").GetString().Should().Be("User 'alice' already exists.");
    }

    [Fact]
    public async Task Register_EmptyName_Returns400()
    {
        var response = await Client.PostAsJsonAsync(Url, new { name = "", password = "secret123" });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("must not be empty");
    }

    [Fact]
    public async Task Register_PasswordTooShort_Returns400()
    {
        var response = await Client.PostAsJsonAsync(Url, new { name = "alice", password = "abc" });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("at least 6 characters");
    }

    [Fact]
    public async Task Register_NameTooLong_Returns400()
    {
        string longName = new('a', 101);

        var response = await Client.PostAsJsonAsync(Url, new { name = longName, password = "secret123" });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("100 characters or fewer");
    }
}
