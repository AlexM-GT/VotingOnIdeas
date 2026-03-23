using FluentValidation;
using VotingOnIdeas.Application.Exceptions;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;

namespace VotingOnIdeas.Application.Ideas;

public sealed class CreateIdeaUseCase
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateIdeaCommand> _validator;

    public CreateIdeaUseCase(
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateIdeaCommand> validator)
    {
        _ideaRepository = ideaRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<IdeaDto> ExecuteAsync(CreateIdeaCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new Exceptions.ValidationException(errors);
        }

        var idea = Idea.Create(command.Title, command.Description, command.UserId);
        await _ideaRepository.AddAsync(idea, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _ideaRepository.GetByIdAsync(idea.Id, cancellationToken);
        return IdeaDto.From(created!);
    }
}
