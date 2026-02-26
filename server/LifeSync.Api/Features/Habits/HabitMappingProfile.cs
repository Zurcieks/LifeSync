using AutoMapper;
using LifeSync.Api.Data.Entities;

namespace LifeSync.Api.Features.Habits;

public class HabitMappingProfile : Profile
{
    public HabitMappingProfile()
    {
        CreateMap<Habit, HabitDto>()
            .ForMember(d => d.CurrentStreak, opt => opt.MapFrom(s => CalculateStreak(s.Entries)))
            .ForMember(d => d.TotalCompletions, opt => opt.MapFrom(s => s.Entries.Count))
            .ForMember(d => d.CompletedToday, opt => opt.MapFrom(s =>
                s.Entries.Any(e => e.Date == DateOnly.FromDateTime(DateTime.UtcNow))));

        CreateMap<HabitEntry, HabitEntryDto>();
    }

    private static int CalculateStreak(ICollection<HabitEntry> entries)
    {
        if (entries.Count == 0)
            return 0;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
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
