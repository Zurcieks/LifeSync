namespace LifeSync.Api.Features.Expenses;

public record ExpenseDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryColor { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record ExpenseSummaryDto(
    decimal TotalAmount,
    int Count,
    List<CategorySummaryDto> ByCategory);

public record CategorySummaryDto(
    Guid CategoryId,
    string CategoryName,
    string CategoryColor,
    decimal Amount,
    int Count);
