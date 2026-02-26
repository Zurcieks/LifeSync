using FluentValidation.TestHelper;
using LifeSync.Api.Features.Workouts.Commands;

namespace LifeSync.Api.Tests.Unit.Validators;

public class LogWorkoutCommandValidatorTests
{
    private readonly LogWorkoutCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new LogWorkoutCommand(Guid.NewGuid(), 45, "Felt strong");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyPlanId_ShouldFail()
    {
        var command = new LogWorkoutCommand(Guid.Empty, 45, "notes");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TrainingPlanId);
    }

    [Fact]
    public void ZeroDuration_ShouldFail()
    {
        var command = new LogWorkoutCommand(Guid.NewGuid(), 0, "notes");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DurationMinutes);
    }

    [Fact]
    public void NotesTooLong_ShouldFail()
    {
        var command = new LogWorkoutCommand(Guid.NewGuid(), 30, new string('a', 501));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }
}
