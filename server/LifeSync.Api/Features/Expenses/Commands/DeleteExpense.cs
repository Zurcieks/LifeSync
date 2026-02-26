using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Expenses.Commands;

public record DeleteExpenseCommand(Guid Id) : IRequest;

public class DeleteExpenseCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser) : IRequestHandler<DeleteExpenseCommand>
{
    public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Expense not found.");

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync(cancellationToken);
    }
}
