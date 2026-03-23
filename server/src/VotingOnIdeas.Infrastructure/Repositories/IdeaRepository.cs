using Microsoft.EntityFrameworkCore;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;
using VotingOnIdeas.Infrastructure.Persistence;

namespace VotingOnIdeas.Infrastructure.Repositories;

public sealed class IdeaRepository : Repository<Idea, Guid>, IIdeaRepository
{
    public IdeaRepository(AppDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<Idea> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(i => i.Votes).OrderByDescending(i => i.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public override async Task<Idea?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(i => i.Votes)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
}
