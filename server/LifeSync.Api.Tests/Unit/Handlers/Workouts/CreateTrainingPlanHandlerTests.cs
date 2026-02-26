using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Workouts;
using LifeSync.Api.Features.Workouts.Commands;
using LifeSync.Api.Features.Auth.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Workouts;

public class CreateTrainingPlanHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly CreateTrainingPlanCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public CreateTrainingPlanHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<WorkoutMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new CreateTrainingPlanCommandHandler(_db, currentUser, mapper);
    }

    [Fact]
    public async Task Handle_ShouldCreatePlanWithExercises()
    {
        var exercises = new List<CreateExerciseDto>
        {
            new("Bench Press", 4, 8, 60, 0),
            new("Overhead Press", 3, 10, 40, 1)
        };
        var command = new CreateTrainingPlanCommand("Push Day", "Chest and shoulders", exercises);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Push Day");
        result.Exercises.Should().HaveCount(2);
        result.Exercises[0].Name.Should().Be("Bench Press");
        result.Exercises[1].Name.Should().Be("Overhead Press");

        var plan = await _db.TrainingPlans.Include(p => p.Exercises).FirstAsync();
        plan.UserId.Should().Be(_userId);
        plan.Exercises.Should().HaveCount(2);
    }

    public void Dispose() => _db.Dispose();
}

internal class FakeCurrentUserService(Guid userId) : ICurrentUserService
{
    public Guid UserId => userId;
}
