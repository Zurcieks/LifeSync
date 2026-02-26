using LifeSync.Api.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.Api.Features.Dashboard;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        return Ok(await mediator.Send(new GetDashboardSummaryQuery()));
    }
}
