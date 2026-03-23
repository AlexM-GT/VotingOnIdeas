using VotingOnIdeas.Domain.Common;
using VotingOnIdeas.Domain.Exceptions;

namespace VotingOnIdeas.Domain.Entities;

public sealed class RefreshToken : Entity<Guid>
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }

    public User? User { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId must not be empty.");
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("ExpiresAt must be in the future.");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke() => IsRevoked = true;
}
