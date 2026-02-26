namespace LifeSync.Api.Data.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public Guid TrainingPlanId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public decimal Weight { get; set; }
    public int OrderIndex { get; set; }

    public TrainingPlan TrainingPlan { get; set; } = null!;
}
