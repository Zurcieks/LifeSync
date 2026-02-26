using FluentAssertions;
using FluentValidation.TestHelper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Commands;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Tests.Unit.Validators;

public class RegisterCommandValidatorTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);
        _validator = new RegisterCommandValidator(_db);
    }

    [Fact]
    public async Task ValidCommand_ShouldPass()
    {
        var command = new RegisterCommand("test@example.com", "Password123!", "John", "Doe");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        var command = new RegisterCommand(email, "Password123!", "John", "Doe");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task DuplicateEmail_ShouldFail()
    {
        _db.Users.Add(new Data.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            PasswordHash = "hash",
            FirstName = "A",
            LastName = "B"
        });
        await _db.SaveChangesAsync();

        var command = new RegisterCommand("existing@example.com", "Password123!", "John", "Doe");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("An account with this email already exists.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public async Task InvalidPassword_ShouldFail(string password)
    {
        var command = new RegisterCommand("test@example.com", password, "John", "Doe");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task EmptyFirstName_ShouldFail()
    {
        var command = new RegisterCommand("test@example.com", "Password123!", "", "Doe");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task EmptyLastName_ShouldFail()
    {
        var command = new RegisterCommand("test@example.com", "Password123!", "John", "");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    public void Dispose() => _db.Dispose();
}
