using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LifeSync.Api.Features.Auth.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)
                ?? httpContextAccessor.HttpContext?.User
                    .FindFirst(JwtRegisteredClaimNames.Sub);

            if (claim is null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return userId;
        }
    }
}
