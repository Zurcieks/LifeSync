using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Workouts;
using LifeSync.Api.Features.Workouts.Commands;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Workouts;

public class LogWorkoutHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly LogWorkoutCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _planId = Guid.NewGuid();

    public LogWorkoutHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<WorkoutMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new LogWorkoutCommandHandler(_db, currentUser, mapper);

        _db.TrainingPlans.Add(new TrainingPlan
        {
            Id = _planId,
            UserId = _userId,
            Name = "Push Day",
            Description = ""
        });
        _db.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidPlan_ShouldCreateLog()
    {
        var command = new LogWorkoutCommand(_planId, 45, "Good session");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.TrainingPlanId.Should().Be(_planId);
        result.DurationMinutes.Should().Be(45);
        result.Notes.Should().Be("Good session");
        result.TrainingPlanName.Should().Be("Push Day");

        (await _db.WorkoutLogs.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_NonExistentPlan_ShouldThrow()
    {
        var command = new LogWorkoutCommand(Guid.NewGuid(), 30, "");
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Training plan not found.");
    }

    [Fact]
    public async Task Handle_OtherUsersPlan_ShouldThrow()
    {
        var otherPlanId = Guid.NewGuid();
        _db.TrainingPlans.Add(new TrainingPlan
        {
            Id = otherPlanId,
            UserId = Guid.NewGuid(),
            Name = "Other Plan"
        });
        await _db.SaveChangesAsync();

        var command = new LogWorkoutCommand(otherPlanId, 30, "");
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    public void Dispose() => _db.Dispose();
}
