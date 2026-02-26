namespace LifeSync.Api.Data.Entities;

public class HabitEntry
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }

    public Habit Habit { get; set; } = null!;
}
