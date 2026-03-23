using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Ideas;

public sealed class GetIdeaByIdUseCase
{
    private readonly IIdeaRepository _ideaRepository;

    public GetIdeaByIdUseCase(IIdeaRepository ideaRepository)
    {
        _ideaRepository = ideaRepository;
    }

    public async Task<IdeaDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idea = await _ideaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Idea), id);

        return IdeaDto.From(idea);
    }
}
