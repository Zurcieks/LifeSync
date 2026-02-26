using AutoMapper;
using LifeSync.Api.Data.Entities;

namespace LifeSync.Api.Features.Workouts;

public class WorkoutMappingProfile : Profile
{
    public WorkoutMappingProfile()
    {
        CreateMap<TrainingPlan, TrainingPlanDto>()
            .ForMember(d => d.TotalWorkouts, opt => opt.MapFrom(s => s.WorkoutLogs.Count));

        CreateMap<Exercise, ExerciseDto>();

        CreateMap<WorkoutLog, WorkoutLogDto>()
            .ForMember(d => d.TrainingPlanName, opt => opt.MapFrom(s => s.TrainingPlan.Name));
    }
}
