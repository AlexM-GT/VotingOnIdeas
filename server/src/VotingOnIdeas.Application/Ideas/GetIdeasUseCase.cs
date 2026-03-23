using VotingOnIdeas.Application.Common;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Ideas;

public sealed class GetIdeasUseCase
{
    private readonly IIdeaRepository _ideaRepository;

    public GetIdeasUseCase(IIdeaRepository ideaRepository)
    {
        _ideaRepository = ideaRepository;
    }

    public async Task<PagedResult<IdeaDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var (items, totalCount) = await _ideaRepository.GetPagedAsync(page, pageSize, cancellationToken);
        var dtos = items.Select(IdeaDto.From).ToList();
        return new PagedResult<IdeaDto>(dtos, totalCount, page, pageSize);
    }
}
