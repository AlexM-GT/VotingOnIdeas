using FluentValidation;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Constants;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Ideas;

public sealed class UpdateIdeaUseCase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateIdeaCommand> _validator;

    public UpdateIdeaUseCase(
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateIdeaCommand> validator)
    {
        _ideaRepository = ideaRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<IdeaDto> ExecuteAsync(UpdateIdeaCommand command, CancellationToken cancellationToken = default)
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

        if (idea.UserId != command.RequestedByUserId && command.RequestedByRole != UserRole.Admin)
            throw new UnauthorizedException("Only the owner can update this idea.");

        idea.Update(command.Title, command.Description);
        _ideaRepository.Update(idea);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return IdeaDto.From(idea);
    }
}
