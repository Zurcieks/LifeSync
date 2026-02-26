using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;

namespace LifeSync.Api.Features.Workouts.Commands;

public record CreateTrainingPlanCommand(
    string Name,
    string Description,
    List<CreateExerciseDto> Exercises) : IRequest<TrainingPlanDto>;

public class CreateTrainingPlanCommandValidator : AbstractValidator<CreateTrainingPlanCommand>
{
    public CreateTrainingPlanCommandValidator()
    {
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

public class CreateTrainingPlanCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<CreateTrainingPlanCommand, TrainingPlanDto>
{
    public async Task<TrainingPlanDto> Handle(CreateTrainingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = new TrainingPlan
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Exercises = request.Exercises.Select((e, i) => new Exercise
            {
                Id = Guid.NewGuid(),
                Name = e.Name,
                Sets = e.Sets,
                Reps = e.Reps,
                Weight = e.Weight,
                OrderIndex = e.OrderIndex
            }).ToList()
        };

        db.TrainingPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<TrainingPlanDto>(plan);
    }
}
