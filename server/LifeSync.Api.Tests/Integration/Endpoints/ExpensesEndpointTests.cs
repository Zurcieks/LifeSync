using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeSync.Api.Features.Categories;
using LifeSync.Api.Features.Categories.Commands;
using LifeSync.Api.Features.Expenses;
using LifeSync.Api.Features.Expenses.Commands;
using LifeSync.Api.Tests.Integration.Helpers;

namespace LifeSync.Api.Tests.Integration.Endpoints;

public class ExpensesEndpointTests : IntegrationTestBase
{
    public ExpensesEndpointTests(TestWebApplicationFactory factory) : base(factory) { }

    private async Task<CategoryDto> CreateCategoryAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/categories",
            new CreateCategoryCommand("Food", "#ef4444"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }

    [Fact]
    public async Task CreateExpense_ShouldReturn201()
    {
        var category = await CreateCategoryAsync();
        var command = new CreateExpenseCommand(25.50m, "Lunch", category.Id, DateOnly.FromDateTime(DateTime.Today));

        var response = await Client.PostAsJsonAsync("/api/expenses", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var expense = await response.Content.ReadFromJsonAsync<ExpenseDto>();
        expense!.Amount.Should().Be(25.50m);
        expense.CategoryName.Should().Be("Food");
    }

    [Fact]
    public async Task CreateExpense_InvalidCategory_ShouldReturn404()
    {
        var command = new CreateExpenseCommand(10m, "Test", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));
        var response = await Client.PostAsJsonAsync("/api/expenses", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateExpense_InvalidAmount_ShouldReturn400()
    {
        var category = await CreateCategoryAsync();
        var command = new CreateExpenseCommand(0m, "Test", category.Id, DateOnly.FromDateTime(DateTime.Today));
        var response = await Client.PostAsJsonAsync("/api/expenses", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetExpenses_WithDateFilter_ShouldFilter()
    {
        var category = await CreateCategoryAsync();

        await Client.PostAsJsonAsync("/api/expenses",
            new CreateExpenseCommand(10m, "Jan", category.Id, new DateOnly(2025, 1, 15)));
        await Client.PostAsJsonAsync("/api/expenses",
            new CreateExpenseCommand(20m, "Feb", category.Id, new DateOnly(2025, 2, 15)));

        var response = await Client.GetAsync("/api/expenses?from=2025-02-01&to=2025-02-28");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expenses = await response.Content.ReadFromJsonAsync<List<ExpenseDto>>();
        expenses!.Should().OnlyContain(e => e.Date >= new DateOnly(2025, 2, 1));
    }

    [Fact]
    public async Task GetExpenses_WithoutAuth_ShouldReturn401()
    {
        var unauthClient = Factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/expenses");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
