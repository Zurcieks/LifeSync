using LifeSync.Api.Features.Workouts.Commands;
using LifeSync.Api.Features.Workouts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.Api.Features.Workouts;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkoutsController(IMediator mediator) : ControllerBase
{
    [HttpGet("plans")]
    public async Task<ActionResult<List<TrainingPlanDto>>> GetPlans()
    {
        return Ok(await mediator.Send(new GetTrainingPlansQuery()));
    }

    [HttpGet("plans/{id:guid}")]
    public async Task<ActionResult<TrainingPlanDto>> GetPlanById(Guid id)
    {
        return Ok(await mediator.Send(new GetTrainingPlanByIdQuery(id)));
    }

    [HttpPost("plans")]
    public async Task<ActionResult<TrainingPlanDto>> CreatePlan(CreateTrainingPlanCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetPlanById), new { id = result.Id }, result);
    }

    [HttpPut("plans/{id:guid}")]
    public async Task<ActionResult<TrainingPlanDto>> UpdatePlan(Guid id, UpdateTrainingPlanCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match body id.");

        return Ok(await mediator.Send(command));
    }

    [HttpDelete("plans/{id:guid}")]
    public async Task<IActionResult> DeletePlan(Guid id)
    {
        await mediator.Send(new DeleteTrainingPlanCommand(id));
        return NoContent();
    }

    [HttpGet("logs")]
    public async Task<ActionResult<List<WorkoutLogDto>>> GetLogs([FromQuery] Guid? trainingPlanId = null)
    {
        return Ok(await mediator.Send(new GetWorkoutLogsQuery(trainingPlanId)));
    }

    [HttpPost("logs")]
    public async Task<ActionResult<WorkoutLogDto>> LogWorkout(LogWorkoutCommand command)
    {
        var result = await mediator.Send(command);
        return Created(string.Empty, result);
    }
}
