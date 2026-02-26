using FluentAssertions;
using FluentValidation.TestHelper;
using LifeSync.Api.Features.Workouts;
using LifeSync.Api.Features.Workouts.Commands;

namespace LifeSync.Api.Tests.Unit.Validators;

public class CreateTrainingPlanCommandValidatorTests
{
    private readonly CreateTrainingPlanCommandValidator _validator = new();

    private static List<CreateExerciseDto> ValidExercises =>
        [new("Bench Press", 4, 8, 60, 0)];

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new CreateTrainingPlanCommand("Push Day", "Upper body", ValidExercises);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var command = new CreateTrainingPlanCommand("", "desc", ValidExercises);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NoExercises_ShouldFail()
    {
        var command = new CreateTrainingPlanCommand("Push Day", "desc", []);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Exercises);
    }

    [Fact]
    public void ExerciseWithEmptyName_ShouldFail()
    {
        var exercises = new List<CreateExerciseDto> { new("", 3, 10, 0, 0) };
        var command = new CreateTrainingPlanCommand("Push Day", "desc", exercises);
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ExerciseWithZeroSets_ShouldFail()
    {
        var exercises = new List<CreateExerciseDto> { new("Bench", 0, 10, 0, 0) };
        var command = new CreateTrainingPlanCommand("Push Day", "desc", exercises);
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ExerciseWithNegativeWeight_ShouldFail()
    {
        var exercises = new List<CreateExerciseDto> { new("Bench", 3, 10, -5, 0) };
        var command = new CreateTrainingPlanCommand("Push Day", "desc", exercises);
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
    }
}
