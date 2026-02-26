using FluentAssertions;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Commands;
using LifeSync.Api.Features.Auth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LifeSync.Api.Tests.Unit.Handlers.Auth;

public class RegisterCommandHandlerTests : IDisposable
{
    private readonly LifeSyncDbContext _db;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
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
        _handler = new RegisterCommandHandler(_db, tokenService);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnTokens()
    {
        var command = new RegisterCommand("test@example.com", "Password123!", "John", "Doe");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be("test@example.com");
        result.User.FirstName.Should().Be("John");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBe("Password123!");
    }

    [Fact]
    public async Task Handle_ShouldStoreRefreshTokenInDb()
    {
        var command = new RegisterCommand("refresh@example.com", "Password123!", "Jane", "Doe");

        var result = await _handler.Handle(command, CancellationToken.None);

        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == result.RefreshToken);
        refreshToken.Should().NotBeNull();
        refreshToken!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldHashPassword()
    {
        var command = new RegisterCommand("hash@example.com", "Password123!", "John", "Doe");

        await _handler.Handle(command, CancellationToken.None);

        var user = await _db.Users.FirstAsync(u => u.Email == "hash@example.com");
        BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash).Should().BeTrue();
    }

    public void Dispose() => _db.Dispose();
}
