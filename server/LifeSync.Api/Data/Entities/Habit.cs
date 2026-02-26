namespace LifeSync.Api.Data.Entities;

public class Habit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; }

    public User User { get; set; } = null!;
    public ICollection<HabitEntry> Entries { get; set; } = [];
}
