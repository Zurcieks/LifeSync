using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Workouts.Commands;

public record LogWorkoutCommand(
    Guid TrainingPlanId,
    int DurationMinutes,
    string Notes) : IRequest<WorkoutLogDto>;

public class LogWorkoutCommandValidator : AbstractValidator<LogWorkoutCommand>
{
    public LogWorkoutCommandValidator()
    {
        RuleFor(x => x.TrainingPlanId).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class LogWorkoutCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<LogWorkoutCommand, WorkoutLogDto>
{
    public async Task<WorkoutLogDto> Handle(LogWorkoutCommand request, CancellationToken cancellationToken)
    {
        var planExists = await db.TrainingPlans
            .AnyAsync(tp => tp.Id == request.TrainingPlanId && tp.UserId == currentUser.UserId, cancellationToken);

        if (!planExists)
            throw new KeyNotFoundException("Training plan not found.");

        var log = new WorkoutLog
        {
            Id = Guid.NewGuid(),
            TrainingPlanId = request.TrainingPlanId,
            CompletedAt = DateTime.UtcNow,
            DurationMinutes = request.DurationMinutes,
            Notes = request.Notes
        };

        db.WorkoutLogs.Add(log);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(log).Reference(l => l.TrainingPlan).LoadAsync(cancellationToken);
        return mapper.Map<WorkoutLogDto>(log);
    }
}
