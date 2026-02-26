using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeSync.Api.Features.Habits;
using LifeSync.Api.Features.Habits.Commands;
using LifeSync.Api.Tests.Integration.Helpers;

namespace LifeSync.Api.Tests.Integration.Endpoints;

public class HabitsEndpointTests : IntegrationTestBase
{
    public HabitsEndpointTests(TestWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateHabit_ShouldReturn201()
    {
        var command = new CreateHabitCommand("Exercise", "30 min daily");
        var response = await Client.PostAsJsonAsync("/api/habits", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var habit = await response.Content.ReadFromJsonAsync<HabitDto>();
        habit!.Name.Should().Be("Exercise");
        habit.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateHabit_EmptyName_ShouldReturn400()
    {
        var command = new CreateHabitCommand("", "desc");
        var response = await Client.PostAsJsonAsync("/api/habits", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetHabits_ShouldReturnCreatedHabits()
    {
        await Client.PostAsJsonAsync("/api/habits", new CreateHabitCommand("Habit A", ""));
        await Client.PostAsJsonAsync("/api/habits", new CreateHabitCommand("Habit B", ""));

        var response = await Client.GetAsync("/api/habits");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var habits = await response.Content.ReadFromJsonAsync<List<HabitDto>>();
        habits!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task ToggleHabitEntry_ShouldToggle()
    {
        var createResponse = await Client.PostAsJsonAsync("/api/habits", new CreateHabitCommand("Toggle Test", ""));
        var habit = await createResponse.Content.ReadFromJsonAsync<HabitDto>();

        var toggleCommand = new ToggleHabitEntryCommand(habit!.Id, DateOnly.FromDateTime(DateTime.UtcNow));
        var response = await Client.PostAsJsonAsync($"/api/habits/{habit.Id}/toggle", toggleCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var toggled = await response.Content.ReadFromJsonAsync<HabitDto>();
        toggled!.TotalCompletions.Should().Be(1);
        toggled.CompletedToday.Should().BeTrue();
    }

    [Fact]
    public async Task GetHabits_WithoutAuth_ShouldReturn401()
    {
        var unauthClient = Factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/habits");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetHabits_OwnershipScoping_ShouldNotSeeOtherUserHabits()
    {
        await Client.PostAsJsonAsync("/api/habits", new CreateHabitCommand("My Habit", ""));

        var otherClient = await TestAuthHelper.CreateAuthenticatedClientAsync(
            Factory, $"other-{Guid.NewGuid():N}@test.com");
        var response = await otherClient.GetAsync("/api/habits");
        var habits = await response.Content.ReadFromJsonAsync<List<HabitDto>>();

        habits!.Should().NotContain(h => h.Name == "My Habit");
    }
}
