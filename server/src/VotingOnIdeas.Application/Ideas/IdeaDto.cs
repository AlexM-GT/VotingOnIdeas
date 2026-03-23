using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Application.Ideas;

public record IdeaDto(
    Guid Id,
    string Title,
    string Description,
    Guid UserId,
    string Username,
    DateTime CreatedAt,
    double? AverageRating,
    int VoteCount)
{
    public static IdeaDto From(Idea idea) => new(
        idea.Id,
        idea.Title,
        idea.Description,
        idea.UserId,
        idea.User?.Username ?? string.Empty,
        idea.CreatedAt,
        idea.Votes.Count == 0 ? null : idea.Votes.Average(v => (double)v.Value),
        idea.Votes.Count);
}
