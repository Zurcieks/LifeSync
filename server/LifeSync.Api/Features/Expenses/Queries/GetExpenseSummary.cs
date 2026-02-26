using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Expenses.Queries;

public record GetExpenseSummaryQuery(
    DateOnly? From = null,
    DateOnly? To = null) : IRequest<ExpenseSummaryDto>;

public class GetExpenseSummaryQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser) : IRequestHandler<GetExpenseSummaryQuery, ExpenseSummaryDto>
{
    public async Task<ExpenseSummaryDto> Handle(GetExpenseSummaryQuery request, CancellationToken cancellationToken)
    {
        var query = db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == currentUser.UserId);

        if (request.From.HasValue)
            query = query.Where(e => e.Date >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(e => e.Date <= request.To.Value);

        var expenses = await query.ToListAsync(cancellationToken);

        var byCategory = expenses
            .GroupBy(e => new { e.CategoryId, e.Category.Name, e.Category.Color })
            .Select(g => new CategorySummaryDto(
                g.Key.CategoryId,
                g.Key.Name,
                g.Key.Color,
                g.Sum(e => e.Amount),
                g.Count()))
            .OrderByDescending(c => c.Amount)
            .ToList();

        return new ExpenseSummaryDto(
            expenses.Sum(e => e.Amount),
            expenses.Count,
            byCategory);
    }
}
