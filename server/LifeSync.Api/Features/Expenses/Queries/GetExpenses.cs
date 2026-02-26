using AutoMapper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Expenses.Queries;

public record GetExpensesQuery(
    DateOnly? From = null,
    DateOnly? To = null,
    Guid? CategoryId = null) : IRequest<List<ExpenseDto>>;

public class GetExpensesQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetExpensesQuery, List<ExpenseDto>>
{
    public async Task<List<ExpenseDto>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == currentUser.UserId);

        if (request.From.HasValue)
            query = query.Where(e => e.Date >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(e => e.Date <= request.To.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == request.CategoryId.Value);

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<ExpenseDto>>(expenses);
    }
}
