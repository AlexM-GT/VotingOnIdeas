using FluentValidation;

namespace VotingOnIdeas.Application.Ideas;

public record CreateIdeaCommand(string Title, string Description, Guid UserId);

public sealed class CreateIdeaCommandValidator : AbstractValidator<CreateIdeaCommand>
{
    public CreateIdeaCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
