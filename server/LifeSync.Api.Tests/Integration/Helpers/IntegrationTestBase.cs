namespace LifeSync.Api.Tests.Integration.Helpers;

public abstract class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected HttpClient Client = null!;

    protected IntegrationTestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
    }

    public virtual async Task InitializeAsync()
    {
        var email = $"user-{Guid.NewGuid():N}@test.com";
        Client = await TestAuthHelper.CreateAuthenticatedClientAsync(Factory, email);
    }

    public virtual Task DisposeAsync()
    {
        Client?.Dispose();
        return Task.CompletedTask;
    }
}
