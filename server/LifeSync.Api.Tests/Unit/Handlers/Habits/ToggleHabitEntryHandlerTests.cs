using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Habits;
using LifeSync.Api.Features.Habits.Commands;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Habits;

public class ToggleHabitEntryHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly ToggleHabitEntryCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _habitId = Guid.NewGuid();

    public ToggleHabitEntryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<HabitMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new ToggleHabitEntryCommandHandler(_db, currentUser, mapper);

        _db.Habits.Add(new Habit
        {
            Id = _habitId,
            UserId = _userId,
            Name = "Test Habit",
            Description = ""
        });
        _db.SaveChanges();
    }

    [Fact]
    public async Task Handle_NoEntry_ShouldAddEntry()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var command = new ToggleHabitEntryCommand(_habitId, date);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.TotalCompletions.Should().Be(1);
        var entry = await _db.HabitEntries.FirstOrDefaultAsync(e => e.HabitId == _habitId && e.Date == date);
        entry.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ExistingEntry_ShouldRemoveEntry()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        _db.HabitEntries.Add(new HabitEntry { Id = Guid.NewGuid(), HabitId = _habitId, Date = date });
        await _db.SaveChangesAsync();

        var command = new ToggleHabitEntryCommand(_habitId, date);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.TotalCompletions.Should().Be(0);
    }

    [Fact]
    public async Task Handle_HabitNotFound_ShouldThrow()
    {
        var command = new ToggleHabitEntryCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow));
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    public void Dispose() => _db.Dispose();
}
