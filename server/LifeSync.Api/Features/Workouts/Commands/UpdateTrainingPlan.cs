using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Workouts.Commands;

public record UpdateTrainingPlanCommand(
    Guid Id,
    string Name,
    string Description,
    List<CreateExerciseDto> Exercises) : IRequest<TrainingPlanDto>;

public class UpdateTrainingPlanCommandValidator : AbstractValidator<UpdateTrainingPlanCommand>
{
    public UpdateTrainingPlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Exercises).NotEmpty().WithMessage("At least one exercise is required.");
        RuleForEach(x => x.Exercises).ChildRules(exercise =>
        {
            exercise.RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
            exercise.RuleFor(e => e.Sets).GreaterThanOrEqualTo(1);
            exercise.RuleFor(e => e.Reps).GreaterThanOrEqualTo(1);
            exercise.RuleFor(e => e.Weight).GreaterThanOrEqualTo(0);
        });
    }
}

public class UpdateTrainingPlanCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<UpdateTrainingPlanCommand, TrainingPlanDto>
{
    public async Task<TrainingPlanDto> Handle(UpdateTrainingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await db.TrainingPlans
            .Include(tp => tp.Exercises)
            .Include(tp => tp.WorkoutLogs)
            .FirstOrDefaultAsync(tp => tp.Id == request.Id && tp.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Training plan not found.");

        plan.Name = request.Name;
        plan.Description = request.Description;

        db.Exercises.RemoveRange(plan.Exercises);
        plan.Exercises = request.Exercises.Select(e => new Exercise
        {
            Id = Guid.NewGuid(),
            TrainingPlanId = plan.Id,
            Name = e.Name,
            Sets = e.Sets,
            Reps = e.Reps,
            Weight = e.Weight,
            OrderIndex = e.OrderIndex
        }).ToList();

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<TrainingPlanDto>(plan);
    }
}
