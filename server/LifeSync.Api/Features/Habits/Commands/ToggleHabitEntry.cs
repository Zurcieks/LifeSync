using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Habits.Commands;

public record ToggleHabitEntryCommand(Guid HabitId, DateOnly Date) : IRequest<HabitDto>;

public class ToggleHabitEntryCommandValidator : AbstractValidator<ToggleHabitEntryCommand>
{
    public ToggleHabitEntryCommandValidator()
    {
        RuleFor(x => x.HabitId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}

public class ToggleHabitEntryCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<ToggleHabitEntryCommand, HabitDto>
{
    public async Task<HabitDto> Handle(ToggleHabitEntryCommand request, CancellationToken cancellationToken)
    {
        var habit = await db.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == request.HabitId && h.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Habit not found.");

        var existing = habit.Entries.FirstOrDefault(e => e.Date == request.Date);

        if (existing is not null)
        {
            db.HabitEntries.Remove(existing);
        }
        else
        {
            db.HabitEntries.Add(new HabitEntry
            {
                Id = Guid.NewGuid(),
                HabitId = habit.Id,
                Date = request.Date
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(habit).Collection(h => h.Entries).LoadAsync(cancellationToken);
        return mapper.Map<HabitDto>(habit);
    }
}
