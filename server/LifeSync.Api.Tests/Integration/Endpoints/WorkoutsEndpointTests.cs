using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeSync.Api.Features.Workouts;
using LifeSync.Api.Features.Workouts.Commands;
using LifeSync.Api.Tests.Integration.Helpers;

namespace LifeSync.Api.Tests.Integration.Endpoints;

public class WorkoutsEndpointTests : IntegrationTestBase
{
    public WorkoutsEndpointTests(TestWebApplicationFactory factory) : base(factory) { }

    private CreateTrainingPlanCommand ValidPlanCommand => new(
        "Push Day",
        "Chest and shoulders",
        [new CreateExerciseDto("Bench Press", 4, 8, 60, 0)]);

    [Fact]
    public async Task CreateTrainingPlan_ShouldReturn201()
    {
        var response = await Client.PostAsJsonAsync("/api/workouts/plans", ValidPlanCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var plan = await response.Content.ReadFromJsonAsync<TrainingPlanDto>();
        plan!.Name.Should().Be("Push Day");
        plan.Exercises.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateTrainingPlan_EmptyName_ShouldReturn400()
    {
        var command = new CreateTrainingPlanCommand("", "desc",
            [new CreateExerciseDto("Bench", 3, 10, 50, 0)]);
        var response = await Client.PostAsJsonAsync("/api/workouts/plans", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTrainingPlans_ShouldReturnCreated()
    {
        await Client.PostAsJsonAsync("/api/workouts/plans", ValidPlanCommand);

        var response = await Client.GetAsync("/api/workouts/plans");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var plans = await response.Content.ReadFromJsonAsync<List<TrainingPlanDto>>();
        plans!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LogWorkout_ShouldReturn201()
    {
        var planResponse = await Client.PostAsJsonAsync("/api/workouts/plans", ValidPlanCommand);
        var plan = await planResponse.Content.ReadFromJsonAsync<TrainingPlanDto>();

        var logCommand = new LogWorkoutCommand(plan!.Id, 45, "Great session");
        var response = await Client.PostAsJsonAsync("/api/workouts/logs", logCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var log = await response.Content.ReadFromJsonAsync<WorkoutLogDto>();
        log!.DurationMinutes.Should().Be(45);
        log.TrainingPlanName.Should().Be("Push Day");
    }

    [Fact]
    public async Task LogWorkout_InvalidPlan_ShouldReturn404()
    {
        var command = new LogWorkoutCommand(Guid.NewGuid(), 30, "");
        var response = await Client.PostAsJsonAsync("/api/workouts/logs", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTrainingPlans_WithoutAuth_ShouldReturn401()
    {
        var unauthClient = Factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/workouts/plans");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
