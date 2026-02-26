using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Categories.Commands;

public record DeleteCategoryCommand(Guid Id) : IRequest;

public class DeleteCategoryCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");

        var hasExpenses = await db.Expenses.AnyAsync(e => e.CategoryId == request.Id, cancellationToken);
        if (hasExpenses)
            throw new InvalidOperationException("Cannot delete a category that has expenses. Reassign or delete the expenses first.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
    }
}
