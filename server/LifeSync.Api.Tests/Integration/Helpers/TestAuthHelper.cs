using System.Net.Http.Json;
using LifeSync.Api.Features.Auth;
using LifeSync.Api.Features.Auth.Commands;

namespace LifeSync.Api.Tests.Integration.Helpers;

public static class TestAuthHelper
{
    public static async Task<AuthResponse> RegisterUserAsync(
        HttpClient client,
        string email = "test@example.com",
        string password = "Password123!",
        string firstName = "Test",
        string lastName = "User")
    {
        var command = new RegisterCommand(email, password, firstName, lastName);
        var response = await client.PostAsJsonAsync("/api/auth/register", command);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        TestWebApplicationFactory factory,
        string email = "test@example.com")
    {
        var client = factory.CreateClient();
        var auth = await RegisterUserAsync(client, email);
        SetBearerToken(client, auth.AccessToken);
        return client;
    }
}
