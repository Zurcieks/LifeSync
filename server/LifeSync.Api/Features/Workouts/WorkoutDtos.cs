namespace LifeSync.Api.Features.Workouts;

public record TrainingPlanDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public List<ExerciseDto> Exercises { get; init; } = [];
    public int TotalWorkouts { get; init; }
}

public record ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Sets { get; init; }
    public int Reps { get; init; }
    public decimal Weight { get; init; }
    public int OrderIndex { get; init; }
}

public record WorkoutLogDto
{
    public Guid Id { get; init; }
    public Guid TrainingPlanId { get; init; }
    public string TrainingPlanName { get; init; } = string.Empty;
    public DateTime CompletedAt { get; init; }
    public int DurationMinutes { get; init; }
    public string Notes { get; init; } = string.Empty;
}

public record CreateExerciseDto(
    string Name,
    int Sets,
    int Reps,
    decimal Weight,
    int OrderIndex);
