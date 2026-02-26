using AutoMapper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Workouts.Queries;

public record GetTrainingPlanByIdQuery(Guid Id) : IRequest<TrainingPlanDto>;

public class GetTrainingPlanByIdQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetTrainingPlanByIdQuery, TrainingPlanDto>
{
    public async Task<TrainingPlanDto> Handle(GetTrainingPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await db.TrainingPlans
            .Include(tp => tp.Exercises.OrderBy(e => e.OrderIndex))
            .Include(tp => tp.WorkoutLogs)
            .FirstOrDefaultAsync(tp => tp.Id == request.Id && tp.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Training plan not found.");

        return mapper.Map<TrainingPlanDto>(plan);
    }
}
