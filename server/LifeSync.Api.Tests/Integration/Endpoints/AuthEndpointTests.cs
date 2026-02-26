using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeSync.Api.Features.Auth;
using LifeSync.Api.Features.Auth.Commands;
using LifeSync.Api.Tests.Integration.Helpers;

namespace LifeSync.Api.Tests.Integration.Endpoints;

public class AuthEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public AuthEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ValidData_ShouldReturn200WithTokens()
    {
        var client = _factory.CreateClient();
        var email = $"reg-{Guid.NewGuid():N}@test.com";
        var command = new RegisterCommand(email, "Password123!", "John", "Doe");

        var response = await client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrWhiteSpace();
        auth.RefreshToken.Should().NotBeNullOrWhiteSpace();
        auth.User.Email.Should().Be(email.ToLowerInvariant());
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturn400()
    {
        var client = _factory.CreateClient();
        var email = $"dup-{Guid.NewGuid():N}@test.com";
        var command = new RegisterCommand(email, "Password123!", "John", "Doe");

        await client.PostAsJsonAsync("/api/auth/register", command);
        var response = await client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_InvalidEmail_ShouldReturn400()
    {
        var client = _factory.CreateClient();
        var command = new RegisterCommand("not-an-email", "Password123!", "John", "Doe");

        var response = await client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnTokens()
    {
        var client = _factory.CreateClient();
        var email = $"login-{Guid.NewGuid():N}@test.com";

        await TestAuthHelper.RegisterUserAsync(client, email, "Password123!");

        var loginCommand = new LoginCommand(email, "Password123!");
        var response = await client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WrongPassword_ShouldReturn401()
    {
        var client = _factory.CreateClient();
        var email = $"wrongpw-{Guid.NewGuid():N}@test.com";

        await TestAuthHelper.RegisterUserAsync(client, email, "Password123!");

        var loginCommand = new LoginCommand(email, "WrongPassword!");
        var response = await client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ValidToken_ShouldReturnNewTokens()
    {
        var client = _factory.CreateClient();
        var email = $"refresh-{Guid.NewGuid():N}@test.com";
        var auth = await TestAuthHelper.RegisterUserAsync(client, email);

        TestAuthHelper.SetBearerToken(client, auth.AccessToken);
        var refreshCommand = new RefreshTokenCommand(auth.RefreshToken);
        var response = await client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var newAuth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        newAuth!.AccessToken.Should().NotBe(auth.AccessToken);
        newAuth.RefreshToken.Should().NotBe(auth.RefreshToken);
    }
}
