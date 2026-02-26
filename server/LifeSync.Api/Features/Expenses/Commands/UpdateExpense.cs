using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Expenses.Commands;

public record UpdateExpenseCommand(
    Guid Id,
    decimal Amount,
    string Description,
    Guid CategoryId,
    DateOnly Date) : IRequest<ExpenseDto>;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(999999.99m);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}

public class UpdateExpenseCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<UpdateExpenseCommand, ExpenseDto>
{
    public async Task<ExpenseDto> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Expense not found.");

        if (request.CategoryId != expense.CategoryId)
        {
            var categoryExists = await db.Categories
                .AnyAsync(c => c.Id == request.CategoryId && c.UserId == currentUser.UserId, cancellationToken);

            if (!categoryExists)
                throw new KeyNotFoundException("Category not found.");
        }

        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.CategoryId = request.CategoryId;
        expense.Date = request.Date;

        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(expense).Reference(e => e.Category).LoadAsync(cancellationToken);
        return mapper.Map<ExpenseDto>(expense);
    }
}
