using LifeSync.Api.Data.Entities;

namespace LifeSync.Api.Features.Auth.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}
