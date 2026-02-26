using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Expenses.Commands;

public record CreateExpenseCommand(
    decimal Amount,
    string Description,
    Guid CategoryId,
    DateOnly Date) : IRequest<ExpenseDto>;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(999999.99m);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}

public class CreateExpenseCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await db.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.UserId == currentUser.UserId, cancellationToken);

        if (!categoryExists)
            throw new KeyNotFoundException("Category not found.");

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            Amount = request.Amount,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        db.Expenses.Add(expense);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(expense).Reference(e => e.Category).LoadAsync(cancellationToken);
        return mapper.Map<ExpenseDto>(expense);
    }
}
