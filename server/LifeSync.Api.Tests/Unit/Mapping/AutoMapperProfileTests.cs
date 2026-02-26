using AutoMapper;
using LifeSync.Api.Features.Categories;
using LifeSync.Api.Features.Expenses;
using LifeSync.Api.Features.Habits;
using LifeSync.Api.Features.Workouts;

namespace LifeSync.Api.Tests.Unit.Mapping;

public class AutoMapperProfileTests
{
    [Fact]
    public void AllProfiles_ShouldHaveValidConfiguration()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<HabitMappingProfile>();
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<ExpenseMappingProfile>();
            cfg.AddProfile<WorkoutMappingProfile>();
        });

        config.AssertConfigurationIsValid();
    }
}
