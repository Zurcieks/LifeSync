using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Habits;
using LifeSync.Api.Features.Habits.Queries;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Habits;

public class GetHabitsHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly GetHabitsQueryHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _otherUserId = Guid.NewGuid();

    public GetHabitsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<HabitMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new GetHabitsQueryHandler(_db, currentUser, mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyCurrentUserHabits()
    {
        _db.Habits.AddRange(
            new Habit { Id = Guid.NewGuid(), UserId = _userId, Name = "My Habit" },
            new Habit { Id = Guid.NewGuid(), UserId = _otherUserId, Name = "Other Habit" }
        );
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetHabitsQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("My Habit");
    }

    [Fact]
    public async Task Handle_ShouldExcludeArchivedByDefault()
    {
        _db.Habits.AddRange(
            new Habit { Id = Guid.NewGuid(), UserId = _userId, Name = "Active" },
            new Habit { Id = Guid.NewGuid(), UserId = _userId, Name = "Archived", IsArchived = true }
        );
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetHabitsQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Active");
    }

    [Fact]
    public async Task Handle_IncludeArchived_ShouldReturnAll()
    {
        _db.Habits.AddRange(
            new Habit { Id = Guid.NewGuid(), UserId = _userId, Name = "Active" },
            new Habit { Id = Guid.NewGuid(), UserId = _userId, Name = "Archived", IsArchived = true }
        );
        await _db.SaveChangesAsync();

        var result = await _handler.Handle(new GetHabitsQuery(IncludeArchived: true), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    public void Dispose() => _db.Dispose();
}
