using AutoMapper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Workouts.Queries;

public record GetTrainingPlansQuery : IRequest<List<TrainingPlanDto>>;

public class GetTrainingPlansQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetTrainingPlansQuery, List<TrainingPlanDto>>
{
    public async Task<List<TrainingPlanDto>> Handle(GetTrainingPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await db.TrainingPlans
            .Include(tp => tp.Exercises.OrderBy(e => e.OrderIndex))
            .Include(tp => tp.WorkoutLogs)
            .Where(tp => tp.UserId == currentUser.UserId)
            .OrderByDescending(tp => tp.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<TrainingPlanDto>>(plans);
    }
}
