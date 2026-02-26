using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Expenses;
using LifeSync.Api.Features.Expenses.Commands;
using LifeSync.Api.Features.Auth.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Expenses;

public class CreateExpenseHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly CreateExpenseCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public CreateExpenseHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<ExpenseMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new CreateExpenseCommandHandler(_db, currentUser, mapper);

        _db.Categories.Add(new Category
        {
            Id = _categoryId,
            UserId = _userId,
            Name = "Food",
            Color = "#ef4444"
        });
        _db.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidCategory_ShouldCreateExpense()
    {
        var command = new CreateExpenseCommand(25.50m, "Lunch", _categoryId, DateOnly.FromDateTime(DateTime.Today));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Amount.Should().Be(25.50m);
        result.Description.Should().Be("Lunch");
        result.CategoryName.Should().Be("Food");

        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Description == "Lunch");
        expense.Should().NotBeNull();
        expense!.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task Handle_InvalidCategory_ShouldThrow()
    {
        var command = new CreateExpenseCommand(10m, "Test", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Category not found.");
    }

    [Fact]
    public async Task Handle_OtherUsersCategory_ShouldThrow()
    {
        var otherCategoryId = Guid.NewGuid();
        _db.Categories.Add(new Category
        {
            Id = otherCategoryId,
            UserId = Guid.NewGuid(),
            Name = "Other",
            Color = "#000000"
        });
        await _db.SaveChangesAsync();

        var command = new CreateExpenseCommand(10m, "Test", otherCategoryId, DateOnly.FromDateTime(DateTime.Today));
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    public void Dispose() => _db.Dispose();
}

internal class FakeCurrentUserService(Guid userId) : ICurrentUserService
{
    public Guid UserId => userId;
}
