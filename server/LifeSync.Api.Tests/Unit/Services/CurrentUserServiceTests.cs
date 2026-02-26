using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using LifeSync.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Http;

namespace LifeSync.Api.Tests.Unit.Services;

public class CurrentUserServiceTests
{
    [Fact]
    public void UserId_WithValidSubClaim_ShouldReturnGuid()
    {
        var userId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ], "test"));

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var sut = new CurrentUserService(accessor);

        sut.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserId_WithJwtSubClaim_ShouldReturnGuid()
    {
        var userId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        ], "test"));

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var sut = new CurrentUserService(accessor);

        sut.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserId_WithNoClaim_ShouldThrowUnauthorized()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var sut = new CurrentUserService(accessor);

        var act = () => { _ = sut.UserId; };
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("User is not authenticated.");
    }

    [Fact]
    public void UserId_WithInvalidGuid_ShouldThrowUnauthorized()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
        ], "test"));

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var sut = new CurrentUserService(accessor);

        var act = () => { _ = sut.UserId; };
        act.Should().Throw<UnauthorizedAccessException>();
    }
}
