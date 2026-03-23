namespace VotingOnIdeas.Application.Auth;

public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);
