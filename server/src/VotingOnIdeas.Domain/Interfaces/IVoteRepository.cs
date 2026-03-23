using VotingOnIdeas.Domain.Common;
using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Domain.Interfaces;

public interface IVoteRepository : IRepository<Vote, Guid>
{
    Task<Vote?> GetByIdeaAndUserAsync(Guid ideaId, Guid userId, CancellationToken cancellationToken = default);
}
