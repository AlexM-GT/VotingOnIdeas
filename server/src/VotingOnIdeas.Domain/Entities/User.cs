using VotingOnIdeas.Domain.Common;
using VotingOnIdeas.Domain.Constants;

namespace VotingOnIdeas.Domain.Entities;

public sealed class User : Entity<Guid>
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Salt { get; private set; } = string.Empty;
    public string Role { get; private set; } = UserRole.User;

    public IReadOnlyCollection<Idea> Ideas => _ideas.AsReadOnly();
    private readonly List<Idea> _ideas = [];

    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();
    private readonly List<Vote> _votes = [];

    private User() { }

    public static User Create(string username, string email, string passwordHash, string salt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(salt);

        return new User
        {
            Id = Guid.NewGuid(),
            Username = username.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Salt = salt,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void AssignRole(string role) => Role = role;
}
