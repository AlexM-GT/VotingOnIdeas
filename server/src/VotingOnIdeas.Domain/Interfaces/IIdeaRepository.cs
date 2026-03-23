using VotingOnIdeas.Domain.Common;
using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Domain.Interfaces;

public interface IIdeaRepository : IRepository<Idea, Guid>
{
    Task<(IReadOnlyList<Idea> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
