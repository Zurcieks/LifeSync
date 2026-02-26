using FluentValidation.TestHelper;
using LifeSync.Api.Features.Categories.Commands;

namespace LifeSync.Api.Tests.Unit.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new CreateCategoryCommand("Food", "#ef4444");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var command = new CreateCategoryCommand("", "#ef4444");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NameTooLong_ShouldFail()
    {
        var command = new CreateCategoryCommand(new string('a', 51), "#ef4444");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("red")]
    [InlineData("#gggggg")]
    [InlineData("#fff")]
    [InlineData("ef4444")]
    public void InvalidColor_ShouldFail(string color)
    {
        var command = new CreateCategoryCommand("Food", color);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }
}
