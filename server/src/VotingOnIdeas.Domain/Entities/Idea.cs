using VotingOnIdeas.Domain.Common;
using VotingOnIdeas.Domain.Exceptions;

namespace VotingOnIdeas.Domain.Entities;

public sealed class Idea : Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    public User? User { get; private set; }

    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();
    private readonly List<Vote> _votes = [];

    private Idea() { }

    public static Idea Create(string title, string description, Guid userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (userId == Guid.Empty)
            throw new DomainException("UserId must not be empty.");

        return new Idea
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description.Trim(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(string title, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Title = title.Trim();
        Description = description.Trim();
    }
}
