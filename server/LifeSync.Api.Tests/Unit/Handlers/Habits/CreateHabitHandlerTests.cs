using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using LifeSync.Api.Features.Habits;
using LifeSync.Api.Features.Habits.Commands;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Habits;

public class CreateHabitHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly CreateHabitCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public CreateHabitHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<HabitMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new CreateHabitCommandHandler(_db, currentUser, mapper);
    }

    [Fact]
    public async Task Handle_ShouldCreateHabitAndReturnDto()
    {
        var command = new CreateHabitCommand("Read Books", "30 min daily");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Read Books");
        result.Description.Should().Be("30 min daily");
        result.Id.Should().NotBeEmpty();

        var habit = await _db.Habits.FirstOrDefaultAsync(h => h.Name == "Read Books");
        habit.Should().NotBeNull();
        habit!.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task Handle_ShouldScopeToCurrentUser()
    {
        var command = new CreateHabitCommand("Exercise", "");
        var result = await _handler.Handle(command, CancellationToken.None);

        var habit = await _db.Habits.FindAsync(result.Id);
        habit!.UserId.Should().Be(_userId);
    }

    public void Dispose() => _db.Dispose();
}

internal class FakeCurrentUserService(Guid userId) : ICurrentUserService
{
    public Guid UserId => userId;
}
