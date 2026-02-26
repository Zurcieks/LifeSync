namespace LifeSync.Api.Data.Entities;

public class TrainingPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<Exercise> Exercises { get; set; } = [];
    public ICollection<WorkoutLog> WorkoutLogs { get; set; } = [];
}
