using FluentAssertions;
using FluentValidation.TestHelper;
using LifeSync.Api.Features.Habits.Commands;

namespace LifeSync.Api.Tests.Unit.Validators;

public class CreateHabitCommandValidatorTests
{
    private readonly CreateHabitCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new CreateHabitCommand("Exercise", "30 minutes daily");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var command = new CreateHabitCommand("", "description");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameTooLong_ShouldFail()
    {
        var command = new CreateHabitCommand(new string('a', 101), "description");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void DescriptionTooLong_ShouldFail()
    {
        var command = new CreateHabitCommand("Exercise", new string('a', 501));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void EmptyDescription_ShouldPass()
    {
        var command = new CreateHabitCommand("Exercise", "");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
