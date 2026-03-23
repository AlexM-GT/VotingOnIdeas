using VotingOnIdeas.Domain.Entities;

namespace VotingOnIdeas.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    (string Value, DateTime ExpiresAt) GenerateRefreshToken();
}
