using LifeSync.Api.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.Api.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
    {
        var result = await mediator.Send(command);
        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RevokeTokenCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}
