using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using Microsoft.Extensions.Configuration;

namespace LifeSync.Api.Tests.Unit.Services;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly User _testUser;

    public TokenServiceTests()
    {
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

        _sut = new TokenService(config);
        _testUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            FirstName = "John",
            LastName = "Doe"
        };
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwt()
    {
        var token = _sut.GenerateAccessToken(_testUser);

        token.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
        jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateAccessToken_ShouldContainUserClaims()
    {
        var token = _sut.GenerateAccessToken(_testUser);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _testUser.Id.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == _testUser.Email);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void GenerateAccessToken_ShouldExpireInConfiguredMinutes()
    {
        var token = _sut.GenerateAccessToken(_testUser);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(15);
        jwt.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidToken()
    {
        var userId = Guid.NewGuid();
        var refreshToken = _sut.GenerateRefreshToken(userId);

        refreshToken.Should().NotBeNull();
        refreshToken.UserId.Should().Be(userId);
        refreshToken.Token.Should().NotBeNullOrWhiteSpace();
        refreshToken.IsActive.Should().BeTrue();
        refreshToken.IsExpired.Should().BeFalse();
        refreshToken.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void GenerateRefreshToken_ShouldExpireInConfiguredDays()
    {
        var refreshToken = _sut.GenerateRefreshToken(Guid.NewGuid());

        var expectedExpiry = DateTime.UtcNow.AddDays(7);
        refreshToken.ExpiresAt.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldProduceUniqueTokens()
    {
        var token1 = _sut.GenerateRefreshToken(Guid.NewGuid());
        var token2 = _sut.GenerateRefreshToken(Guid.NewGuid());

        token1.Token.Should().NotBe(token2.Token);
    }
}
