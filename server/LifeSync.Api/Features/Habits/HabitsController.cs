using LifeSync.Api.Features.Habits.Commands;
using LifeSync.Api.Features.Habits.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.Api.Features.Habits;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<HabitDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        return Ok(await mediator.Send(new GetHabitsQuery(includeArchived)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HabitDto>> GetById(Guid id)
    {
        return Ok(await mediator.Send(new GetHabitByIdQuery(id)));
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> Create(CreateHabitCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<HabitDto>> Update(Guid id, UpdateHabitCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match body id.");

        return Ok(await mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteHabitCommand(id));
        return NoContent();
    }

    [HttpPost("{habitId:guid}/toggle")]
    public async Task<ActionResult<HabitDto>> ToggleEntry(Guid habitId, ToggleHabitEntryCommand command)
    {
        if (habitId != command.HabitId)
            return BadRequest("Route id does not match body habitId.");

        return Ok(await mediator.Send(command));
    }
}
