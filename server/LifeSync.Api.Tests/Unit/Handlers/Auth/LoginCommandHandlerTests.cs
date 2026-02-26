using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Commands;
using LifeSync.Api.Features.Auth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LifeSync.Api.Tests.Unit.Handlers.Auth;

public class LoginCommandHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<LifeSyncDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new LifeSyncDbContext(options);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestKeyThatIsAtLeast32BytesLongForHmacSha256!!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:AccessTokenExpiresInMinutes"] = "15",
                ["Jwt:RefreshTokenExpiresInDays"] = "7"
            })
            .Build();

        var tokenService = new TokenService(config);
        _handler = new LoginCommandHandler(_db, tokenService);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnTokens()
    {
        await SeedUser("user@test.com", "Password123!");

        var command = new LoginCommand("user@test.com", "Password123!");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task Handle_WrongPassword_ShouldThrowUnauthorized()
    {
        await SeedUser("user@test.com", "Password123!");

        var command = new LoginCommand("user@test.com", "WrongPassword!");
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldThrowUnauthorized()
    {
        var command = new LoginCommand("nobody@test.com", "Password123!");
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    private async Task SeedUser(string email, string password)
    {
        _db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FirstName = "Test",
            LastName = "User"
        });
        await _db.SaveChangesAsync();
    }

    public void Dispose() => _db.Dispose();
}
