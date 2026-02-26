namespace LifeSync.Api.Features.Habits;

public record HabitDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsArchived { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CurrentStreak { get; init; }
    public int TotalCompletions { get; init; }
    public bool CompletedToday { get; init; }
}

public record HabitEntryDto
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
}
