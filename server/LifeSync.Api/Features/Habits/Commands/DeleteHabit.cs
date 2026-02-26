using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Habits.Commands;

public record DeleteHabitCommand(Guid Id) : IRequest;

public class DeleteHabitCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser) : IRequestHandler<DeleteHabitCommand>
{
    public async Task Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
    {
        var habit = await db.Habits
            .FirstOrDefaultAsync(h => h.Id == request.Id && h.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Habit not found.");

        db.Habits.Remove(habit);
        await db.SaveChangesAsync(cancellationToken);
    }
}
