using FluentValidation;

namespace VotingOnIdeas.Application.Ideas;

public record UpdateIdeaCommand(Guid IdeaId, string Title, string Description, Guid RequestedByUserId, string RequestedByRole);

public sealed class UpdateIdeaCommandValidator : AbstractValidator<UpdateIdeaCommand>
{
    public UpdateIdeaCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
