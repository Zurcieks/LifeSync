using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeSync.Api.Features.Dashboard;
using LifeSync.Api.Tests.Integration.Helpers;

namespace LifeSync.Api.Tests.Integration.Endpoints;

public class DashboardEndpointTests : IntegrationTestBase
{
    public DashboardEndpointTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetDashboard_Authenticated_ShouldReturn200()
    {
        var response = await Client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashboard = await response.Content.ReadFromJsonAsync<DashboardSummaryDto>();
        dashboard.Should().NotBeNull();
        dashboard!.Habits.Should().NotBeNull();
        dashboard.Expenses.Should().NotBeNull();
        dashboard.Workouts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDashboard_WithoutAuth_ShouldReturn401()
    {
        var unauthClient = Factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/dashboard");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
