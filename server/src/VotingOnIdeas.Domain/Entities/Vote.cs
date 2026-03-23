using VotingOnIdeas.Domain.Common;
using VotingOnIdeas.Domain.Exceptions;

namespace VotingOnIdeas.Domain.Entities;

public sealed class Vote : Entity<Guid>
{
    public const int MinValue = 1;
    public const int MaxValue = 5;

    public Guid IdeaId { get; private set; }
    public Guid UserId { get; private set; }
    public int Value { get; private set; }

    public Idea? Idea { get; private set; }
    public User? User { get; private set; }

    private Vote() { }

    public static Vote Create(Guid ideaId, Guid userId, int value)
    {
        if (ideaId == Guid.Empty)
            throw new DomainException("IdeaId must not be empty.");
        if (userId == Guid.Empty)
            throw new DomainException("UserId must not be empty.");
        ValidateValue(value);

        return new Vote
        {
            Id = Guid.NewGuid(),
            IdeaId = ideaId,
            UserId = userId,
            Value = value,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void UpdateValue(int value)
    {
        ValidateValue(value);
        Value = value;
    }

    private static void ValidateValue(int value)
    {
        if (value < MinValue || value > MaxValue)
            throw new DomainException($"Vote value must be between {MinValue} and {MaxValue}.");
    }
}
