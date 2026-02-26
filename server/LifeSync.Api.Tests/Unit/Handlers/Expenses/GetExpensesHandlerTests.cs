using AutoMapper;
using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Expenses;
using LifeSync.Api.Features.Expenses.Queries;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Handlers.Expenses;

public class GetExpensesHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly GetExpensesQueryHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public GetExpensesHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<ExpenseMappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        var currentUser = new FakeCurrentUserService(_userId);
        _handler = new GetExpensesQueryHandler(_db, currentUser, mapper);

        _db.Categories.Add(new Category
        {
            Id = _categoryId,
            UserId = _userId,
            Name = "Food",
            Color = "#ef4444"
        });

        _db.Expenses.AddRange(
            new Expense
            {
                Id = Guid.NewGuid(), UserId = _userId, Amount = 10m,
                Description = "Jan", CategoryId = _categoryId,
                Date = new DateOnly(2026, 1, 15)
            },
            new Expense
            {
                Id = Guid.NewGuid(), UserId = _userId, Amount = 20m,
                Description = "Feb", CategoryId = _categoryId,
                Date = new DateOnly(2026, 2, 15)
            },
            new Expense
            {
                Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Amount = 99m,
                Description = "Other", CategoryId = _categoryId,
                Date = new DateOnly(2026, 1, 15)
            }
        );
        _db.SaveChanges();
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyCurrentUserExpenses()
    {
        var result = await _handler.Handle(new GetExpensesQuery(), CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithDateFilter_ShouldFilterCorrectly()
    {
        var query = new GetExpensesQuery(
            From: new DateOnly(2026, 2, 1),
            To: new DateOnly(2026, 2, 28));

        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().HaveCount(1);
        result[0].Description.Should().Be("Feb");
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ShouldFilterCorrectly()
    {
        var otherCategoryId = Guid.NewGuid();
        _db.Categories.Add(new Category
        {
            Id = otherCategoryId, UserId = _userId, Name = "Transport", Color = "#3b82f6"
        });
        _db.Expenses.Add(new Expense
        {
            Id = Guid.NewGuid(), UserId = _userId, Amount = 5m,
            Description = "Bus", CategoryId = otherCategoryId,
            Date = new DateOnly(2026, 1, 20)
        });
        await _db.SaveChangesAsync();

        var query = new GetExpensesQuery(CategoryId: otherCategoryId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Description.Should().Be("Bus");
    }

    public void Dispose() => _db.Dispose();
}
