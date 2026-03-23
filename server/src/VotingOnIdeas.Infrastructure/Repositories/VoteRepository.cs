using Microsoft.EntityFrameworkCore;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;
using VotingOnIdeas.Infrastructure.Persistence;

namespace VotingOnIdeas.Infrastructure.Repositories;

public sealed class VoteRepository : Repository<Vote, Guid>, IVoteRepository
{
    public VoteRepository(AppDbContext context) : base(context) { }

    public Task<Vote?> GetByIdeaAndUserAsync(Guid ideaId, Guid userId, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userId, cancellationToken);
}
