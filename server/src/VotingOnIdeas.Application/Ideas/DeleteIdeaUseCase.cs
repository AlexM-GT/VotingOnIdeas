using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Constants;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Ideas;

public sealed class DeleteIdeaUseCase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIdeaUseCase(IIdeaRepository ideaRepository, IUnitOfWork unitOfWork)
    {
        _ideaRepository = ideaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(DeleteIdeaCommand command, CancellationToken cancellationToken = default)
    {
        var idea = await _ideaRepository.GetByIdAsync(command.IdeaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Idea), command.IdeaId);

        if (idea.UserId != command.RequestedByUserId && command.RequestedByRole != UserRole.Admin)
            throw new UnauthorizedException("Only the owner can delete this idea.");

        _ideaRepository.Remove(idea);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
