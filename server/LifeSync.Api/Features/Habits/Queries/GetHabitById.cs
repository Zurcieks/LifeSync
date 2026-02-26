using AutoMapper;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Habits.Queries;

public record GetHabitByIdQuery(Guid Id) : IRequest<HabitDto>;

public class GetHabitByIdQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetHabitByIdQuery, HabitDto>
{
    public async Task<HabitDto> Handle(GetHabitByIdQuery request, CancellationToken cancellationToken)
    {
        var habit = await db.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == request.Id && h.UserId == currentUser.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Habit not found.");

        return mapper.Map<HabitDto>(habit);
    }
}
