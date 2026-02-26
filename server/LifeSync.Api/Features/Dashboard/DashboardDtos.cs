namespace LifeSync.Api.Features.Dashboard;

public record DashboardSummaryDto(
    HabitsSummaryDto Habits,
    ExpensesSummaryDto Expenses,
    WorkoutsSummaryDto Workouts);

public record HabitsSummaryDto(
    int TotalHabits,
    int CompletedToday,
    int TotalToday,
    int BestCurrentStreak,
    string BestStreakHabitName);

public record ExpensesSummaryDto(
    decimal MonthTotal,
    int MonthCount,
    string TopCategoryName,
    decimal TopCategoryAmount);

public record WorkoutsSummaryDto(
    int WorkoutsThisWeek,
    int TotalPlans,
    string LastPlanName,
    DateTime? LastWorkoutDate);
