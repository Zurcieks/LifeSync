using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Data.Entities;
using LifeSync.Api.Features.Auth.Services;
using MediatR;

namespace LifeSync.Api.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, string Color) : IRequest<CategoryDto>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(7)
            .Matches(@"^#[0-9a-fA-F]{6}$").WithMessage("Color must be a valid hex color (e.g. #ff0000).");
    }
}

public class CreateCategoryCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            Name = request.Name,
            Color = request.Color
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryDto>(category);
    }
}
