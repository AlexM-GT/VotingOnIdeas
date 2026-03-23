namespace VotingOnIdeas.Application.Ideas;

public record DeleteIdeaCommand(Guid IdeaId, Guid RequestedByUserId, string RequestedByRole);
