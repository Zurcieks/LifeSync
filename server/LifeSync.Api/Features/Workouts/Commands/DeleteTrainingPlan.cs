using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Workouts.Commands;

public record DeleteTrainingPlanCommand(Guid Id) : IRequest;

public class DeleteTrainingPlanCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser) : IRequestHandler<DeleteTrainingPlanCommand>
{
    public async Task Handle(DeleteTrainingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await db.TrainingPlans
            .FirstOrDefaultAsync(tp => tp.Id == request.Id && tp.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Training plan not found.");

        db.TrainingPlans.Remove(plan);
        await db.SaveChangesAsync(cancellationToken);
    }
}
