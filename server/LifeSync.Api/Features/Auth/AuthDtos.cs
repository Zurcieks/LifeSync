namespace LifeSync.Api.Features.Auth;

public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);

public record UserDto(Guid Id, string Email, string FirstName, string LastName);
