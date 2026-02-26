using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Habits.Commands;

public record UpdateHabitCommand(Guid Id, string Name, string Description, bool IsArchived) : IRequest<HabitDto>;

public class UpdateHabitCommandValidator : AbstractValidator<UpdateHabitCommand>
{
    public UpdateHabitCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateHabitCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<UpdateHabitCommand, HabitDto>
{
    public async Task<HabitDto> Handle(UpdateHabitCommand request, CancellationToken cancellationToken)
    {
        var habit = await db.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == request.Id && h.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Habit not found.");

        habit.Name = request.Name;
        habit.Description = request.Description;
        habit.IsArchived = request.IsArchived;

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<HabitDto>(habit);
    }
}
