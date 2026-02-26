using AutoMapper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Workouts.Queries;

public record GetWorkoutLogsQuery(Guid? TrainingPlanId = null) : IRequest<List<WorkoutLogDto>>;

public class GetWorkoutLogsQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetWorkoutLogsQuery, List<WorkoutLogDto>>
{
    public async Task<List<WorkoutLogDto>> Handle(GetWorkoutLogsQuery request, CancellationToken cancellationToken)
    {
        var query = db.WorkoutLogs
            .Include(wl => wl.TrainingPlan)
            .Where(wl => wl.TrainingPlan.UserId == currentUser.UserId);

        if (request.TrainingPlanId.HasValue)
            query = query.Where(wl => wl.TrainingPlanId == request.TrainingPlanId.Value);

        var logs = await query
            .OrderByDescending(wl => wl.CompletedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<WorkoutLogDto>>(logs);
    }
}
