using AutoMapper;
using AutoMapper.QueryableExtensions;
using LifeSync.Api.Data;
using LifeSync.Api.Features.Auth.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.Api.Features.Habits.Queries;

public record GetHabitsQuery(bool IncludeArchived = false) : IRequest<List<HabitDto>>;

public class GetHabitsQueryHandler(
    LifeSyncDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetHabitsQuery, List<HabitDto>>
{
    public async Task<List<HabitDto>> Handle(GetHabitsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Habits
            .Include(h => h.Entries)
            .Where(h => h.UserId == currentUser.UserId);

        if (!request.IncludeArchived)
            query = query.Where(h => !h.IsArchived);

        var habits = await query
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<HabitDto>>(habits);
    }
}
