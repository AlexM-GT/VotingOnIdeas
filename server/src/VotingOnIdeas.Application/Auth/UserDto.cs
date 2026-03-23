namespace VotingOnIdeas.Application.Auth;

public record UserDto(Guid Id, string Username, string Email, string Role);
