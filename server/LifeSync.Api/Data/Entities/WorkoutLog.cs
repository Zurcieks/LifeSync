namespace LifeSync.Api.Data.Entities;

public class WorkoutLog
{
    public Guid Id { get; set; }
    public Guid TrainingPlanId { get; set; }
    public DateTime CompletedAt { get; set; }
    public int DurationMinutes { get; set; }
    public string Notes { get; set; } = string.Empty;

    public TrainingPlan TrainingPlan { get; set; } = null!;
}
