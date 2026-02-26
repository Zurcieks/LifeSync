using LifeSync.Api.Features.Expenses.Commands;
using LifeSync.Api.Features.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.Api.Features.Expenses;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ExpenseDto>>> GetAll(
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        [FromQuery] Guid? categoryId = null)
    {
        return Ok(await mediator.Send(new GetExpensesQuery(from, to, categoryId)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> GetById(Guid id)
    {
        return Ok(await mediator.Send(new GetExpenseByIdQuery(id)));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryDto>> GetSummary(
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null)
    {
        return Ok(await mediator.Send(new GetExpenseSummaryQuery(from, to)));
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create(CreateExpenseCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> Update(Guid id, UpdateExpenseCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match body id.");

        return Ok(await mediator.Send(command));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteExpenseCommand(id));
        return NoContent();
    }
}
