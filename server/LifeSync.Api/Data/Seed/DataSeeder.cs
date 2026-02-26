using LifeSync.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(LifeSyncDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "demo@lifesync.dev",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            FirstName = "Demo",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);

        // ── Categories ───────────────────────────────────────────────────────
        var foodId = Guid.NewGuid();
        var transportId = Guid.NewGuid();
        var entertainmentId = Guid.NewGuid();
        var healthId = Guid.NewGuid();
        var shoppingId = Guid.NewGuid();

        var categories = new List<Category>
        {
            new() { Id = foodId, UserId = userId, Name = "Food & Dining", Color = "#ef4444" },
            new() { Id = transportId, UserId = userId, Name = "Transport", Color = "#3b82f6" },
            new() { Id = entertainmentId, UserId = userId, Name = "Entertainment", Color = "#a855f7" },
            new() { Id = healthId, UserId = userId, Name = "Health", Color = "#22c55e" },
            new() { Id = shoppingId, UserId = userId, Name = "Shopping", Color = "#f59e0b" }
        };
        db.Categories.AddRange(categories);

        // ── Expenses (spread across the current month) ───────────────────────
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expenses = new List<Expense>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = foodId, Amount = 12.50m, Description = "Lunch at café", Date = today.AddDays(-1) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = foodId, Amount = 45.00m, Description = "Grocery shopping", Date = today.AddDays(-3) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = transportId, Amount = 30.00m, Description = "Monthly bus pass", Date = today.AddDays(-7) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = transportId, Amount = 15.00m, Description = "Uber ride", Date = today.AddDays(-2) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = entertainmentId, Amount = 14.99m, Description = "Netflix subscription", Date = today.AddDays(-10) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = entertainmentId, Amount = 25.00m, Description = "Movie tickets", Date = today.AddDays(-5) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = healthId, Amount = 40.00m, Description = "Gym membership", Date = today.AddDays(-14) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = healthId, Amount = 22.00m, Description = "Vitamins", Date = today.AddDays(-4) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = shoppingId, Amount = 65.00m, Description = "Running shoes", Date = today.AddDays(-6) },
            new() { Id = Guid.NewGuid(), UserId = userId, CategoryId = foodId, Amount = 8.50m, Description = "Coffee beans", Date = today }
        };
        db.Expenses.AddRange(expenses);

        // ── Habits ───────────────────────────────────────────────────────────
        var exerciseHabitId = Guid.NewGuid();
        var readHabitId = Guid.NewGuid();
        var meditateHabitId = Guid.NewGuid();
        var waterHabitId = Guid.NewGuid();
        var journalHabitId = Guid.NewGuid();

        var habits = new List<Habit>
        {
            new() { Id = exerciseHabitId, UserId = userId, Name = "Exercise", Description = "At least 30 minutes of physical activity" },
            new() { Id = readHabitId, UserId = userId, Name = "Read", Description = "Read for 20 minutes" },
            new() { Id = meditateHabitId, UserId = userId, Name = "Meditate", Description = "10 minutes of mindfulness" },
            new() { Id = waterHabitId, UserId = userId, Name = "Drink Water", Description = "Drink at least 8 glasses of water" },
            new() { Id = journalHabitId, UserId = userId, Name = "Journal", Description = "Write a daily journal entry" }
        };
        db.Habits.AddRange(habits);

        // Habit entries — create streaks for demo purposes
        var entries = new List<HabitEntry>();
        for (var i = 0; i < 21; i++)
        {
            var date = today.AddDays(-i);
            // Exercise: 14-day streak
            if (i < 14)
                entries.Add(new HabitEntry { Id = Guid.NewGuid(), HabitId = exerciseHabitId, Date = date });
            // Read: 7-day streak
            if (i < 7)
                entries.Add(new HabitEntry { Id = Guid.NewGuid(), HabitId = readHabitId, Date = date });
            // Meditate: 10-day streak with a gap at day 5
            if (i < 10 && i != 5)
                entries.Add(new HabitEntry { Id = Guid.NewGuid(), HabitId = meditateHabitId, Date = date });
            // Water: every other day for 20 days
            if (i % 2 == 0)
                entries.Add(new HabitEntry { Id = Guid.NewGuid(), HabitId = waterHabitId, Date = date });
            // Journal: last 3 days
            if (i < 3)
                entries.Add(new HabitEntry { Id = Guid.NewGuid(), HabitId = journalHabitId, Date = date });
        }
        db.HabitEntries.AddRange(entries);

        // ── Training Plans ───────────────────────────────────────────────────
        var pushPullId = Guid.NewGuid();
        var cardioId = Guid.NewGuid();

        var plans = new List<TrainingPlan>
        {
            new()
            {
                Id = pushPullId, UserId = userId,
                Name = "Push/Pull Strength",
                Description = "Upper body strength training alternating push and pull movements"
            },
            new()
            {
                Id = cardioId, UserId = userId,
                Name = "Cardio & Core",
                Description = "Cardiovascular endurance with core strengthening"
            }
        };
        db.TrainingPlans.AddRange(plans);

        var exercises = new List<Exercise>
        {
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, Name = "Bench Press", Sets = 4, Reps = 8, Weight = 60, OrderIndex = 0 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, Name = "Overhead Press", Sets = 3, Reps = 10, Weight = 35, OrderIndex = 1 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, Name = "Barbell Row", Sets = 4, Reps = 8, Weight = 50, OrderIndex = 2 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, Name = "Pull-ups", Sets = 3, Reps = 10, Weight = 0, OrderIndex = 3 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, Name = "Dumbbell Flyes", Sets = 3, Reps = 12, Weight = 14, OrderIndex = 4 },

            new() { Id = Guid.NewGuid(), TrainingPlanId = cardioId, Name = "Treadmill Run", Sets = 1, Reps = 1, Weight = 0, OrderIndex = 0 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = cardioId, Name = "Plank", Sets = 3, Reps = 1, Weight = 0, OrderIndex = 1 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = cardioId, Name = "Russian Twists", Sets = 3, Reps = 20, Weight = 8, OrderIndex = 2 },
            new() { Id = Guid.NewGuid(), TrainingPlanId = cardioId, Name = "Mountain Climbers", Sets = 3, Reps = 30, Weight = 0, OrderIndex = 3 }
        };
        db.Exercises.AddRange(exercises);

        // ── Workout Logs ─────────────────────────────────────────────────────
        var logs = new List<WorkoutLog>
        {
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, CompletedAt = DateTime.UtcNow.AddDays(-1), DurationMinutes = 55, Notes = "Felt strong, increased bench press weight" },
            new() { Id = Guid.NewGuid(), TrainingPlanId = cardioId, CompletedAt = DateTime.UtcNow.AddDays(-2), DurationMinutes = 40, Notes = "Good cardio session" },
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, CompletedAt = DateTime.UtcNow.AddDays(-4), DurationMinutes = 50, Notes = "Standard session" },
            new() { Id = Guid.NewGuid(), TrainingPlanId = cardioId, CompletedAt = DateTime.UtcNow.AddDays(-5), DurationMinutes = 35, Notes = "Quick core focus" },
            new() { Id = Guid.NewGuid(), TrainingPlanId = pushPullId, CompletedAt = DateTime.UtcNow.AddDays(-7), DurationMinutes = 60, Notes = "Long session with extra sets" }
        };
        db.WorkoutLogs.AddRange(logs);

        await db.SaveChangesAsync();
    }
}
