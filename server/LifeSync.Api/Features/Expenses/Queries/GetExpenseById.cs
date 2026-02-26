using AutoMapper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Expenses.Queries;

public record GetExpenseByIdQuery(Guid Id) : IRequest<ExpenseDto>;

public class GetExpenseByIdQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetExpenseByIdQuery, ExpenseDto>
{
    public async Task<ExpenseDto> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var expense = await db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Expense not found.");

        return mapper.Map<ExpenseDto>(expense);
    }
}
