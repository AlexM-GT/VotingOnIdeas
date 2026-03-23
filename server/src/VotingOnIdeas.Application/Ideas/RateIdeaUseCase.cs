using FluentValidation;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Ideas;

public sealed class RateIdeaUseCase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RateIdeaCommand> _validator;

    public RateIdeaUseCase(
        IIdeaRepository ideaRepository,
        IVoteRepository voteRepository,
        IUnitOfWork unitOfWork,
        IValidator<RateIdeaCommand> validator)
    {
        _ideaRepository = ideaRepository;
        _voteRepository = voteRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<IdeaDto> ExecuteAsync(RateIdeaCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException(errors);
        }

        var idea = await _ideaRepository.GetByIdAsync(command.IdeaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Idea), command.IdeaId);

        var existingVote = await _voteRepository.GetByIdeaAndUserAsync(command.IdeaId, command.UserId, cancellationToken);
        if (existingVote is not null)
        {
            existingVote.UpdateValue(command.Value);
            _voteRepository.Update(existingVote);
        }
        else
        {
            var vote = Vote.Create(command.IdeaId, command.UserId, command.Value);
            await _voteRepository.AddAsync(vote, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _ideaRepository.GetByIdAsync(idea.Id, cancellationToken);
        return IdeaDto.From(updated!);
    }
}
