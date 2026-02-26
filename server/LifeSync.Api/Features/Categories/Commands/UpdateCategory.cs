using AutoMapper;
using FluentValidation;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Categories.Commands;

public record UpdateCategoryCommand(Guid Id, string Name, string Color) : IRequest<CategoryDto>;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(7)
            .Matches(@"^#[0-9a-fA-F]{6}$").WithMessage("Color must be a valid hex color (e.g. #ff0000).");
    }
}

public class UpdateCategoryCommandHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");

        category.Name = request.Name;
        category.Color = request.Color;

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryDto>(category);
    }
}
