using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Dashboard.Queries;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var habits = await GetHabitsSummary(userId, cancellationToken);
        var expenses = await GetExpensesSummary(userId, cancellationToken);
        var workouts = await GetWorkoutsSummary(userId, cancellationToken);

        return new DashboardSummaryDto(habits, expenses, workouts);
    }

    private async Task<HabitsSummaryDto> GetHabitsSummary(Guid userId, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var habits = await db.Habits
            .Include(h => h.Entries)
            .Where(h => h.UserId == userId && !h.IsArchived)
            .ToListAsync(ct);

        var completedToday = habits.Count(h => h.Entries.Any(e => e.Date == today));

        var bestStreak = 0;
        var bestStreakName = "—";

        foreach (var habit in habits)
        {
            var streak = CalculateStreak(habit.Entries, today);
            if (streak > bestStreak)
            {
                bestStreak = streak;
                bestStreakName = habit.Name;
            }
        }

        return new HabitsSummaryDto(habits.Count, completedToday, habits.Count, bestStreak, bestStreakName);
    }

    private async Task<ExpensesSummaryDto> GetExpensesSummary(Guid userId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateOnly(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var monthExpenses = await db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.Date >= monthStart && e.Date <= monthEnd)
            .ToListAsync(ct);

        var monthTotal = monthExpenses.Sum(e => e.Amount);
        var monthCount = monthExpenses.Count;

        var topCategory = monthExpenses
            .GroupBy(e => e.Category.Name)
            .OrderByDescending(g => g.Sum(e => e.Amount))
            .FirstOrDefault();

        return new ExpensesSummaryDto(
            monthTotal,
            monthCount,
            topCategory?.Key ?? "—",
            topCategory?.Sum(e => e.Amount) ?? 0);
    }

    private async Task<WorkoutsSummaryDto> GetWorkoutsSummary(Guid userId, CancellationToken ct)
    {
        var weekStart = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        var weekStartDate = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day, 0, 0, 0, DateTimeKind.Utc);

        var totalPlans = await db.TrainingPlans.CountAsync(tp => tp.UserId == userId, ct);

        var workoutsThisWeek = await db.WorkoutLogs
            .CountAsync(wl => wl.TrainingPlan.UserId == userId && wl.CompletedAt >= weekStartDate, ct);

        var lastLog = await db.WorkoutLogs
            .Include(wl => wl.TrainingPlan)
            .Where(wl => wl.TrainingPlan.UserId == userId)
            .OrderByDescending(wl => wl.CompletedAt)
            .FirstOrDefaultAsync(ct);

        return new WorkoutsSummaryDto(
            workoutsThisWeek,
            totalPlans,
            lastLog?.TrainingPlan.Name ?? "—",
            lastLog?.CompletedAt);
    }

    private static int CalculateStreak(ICollection<HabitEntry> entries, DateOnly today)
    {
        if (entries.Count == 0)
            return 0;

        var dates = entries.Select(e => e.Date).OrderByDescending(d => d).ToList();

        if (dates[0] != today && dates[0] != today.AddDays(-1))
            return 0;

        var streak = 1;
        for (var i = 1; i < dates.Count; i++)
        {
            if (dates[i] == dates[i - 1].AddDays(-1))
                streak++;
            else
                break;
        }

        return streak;
    }
}
