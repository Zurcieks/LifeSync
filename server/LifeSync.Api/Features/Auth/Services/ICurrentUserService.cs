namespace LifeSync.Api.Features.Auth.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
