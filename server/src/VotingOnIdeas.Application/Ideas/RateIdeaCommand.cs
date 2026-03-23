using FluentValidation;
using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Application.Ideas;

public record RateIdeaCommand(Guid IdeaId, int Value, Guid UserId);

public sealed class RateIdeaCommandValidator : AbstractValidator<RateIdeaCommand>
{
    public RateIdeaCommandValidator()
    {
        RuleFor(x => x.Value).InclusiveBetween(Vote.MinValue, Vote.MaxValue);
    }
}
