using FluentValidation.TestHelper;
using LifeSync.Api.Features.Expenses.Commands;

namespace LifeSync.Api.Tests.Unit.Validators;

public class CreateExpenseCommandValidatorTests
{
    private readonly CreateExpenseCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new CreateExpenseCommand(25.50m, "Lunch", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void InvalidAmount_ShouldFail(decimal amount)
    {
        var command = new CreateExpenseCommand(amount, "Test", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void AmountTooLarge_ShouldFail()
    {
        var command = new CreateExpenseCommand(1_000_000m, "Test", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void EmptyCategoryId_ShouldFail()
    {
        var command = new CreateExpenseCommand(10m, "Test", Guid.Empty, DateOnly.FromDateTime(DateTime.Today));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void DescriptionTooLong_ShouldFail()
    {
        var command = new CreateExpenseCommand(10m, new string('a', 201), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
