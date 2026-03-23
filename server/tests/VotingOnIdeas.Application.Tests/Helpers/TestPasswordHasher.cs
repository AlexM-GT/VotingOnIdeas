using VotingOnIdeas.Application.Interfaces;

namespace VotingOnIdeas.Application.Tests.Helpers;

internal sealed class TestPasswordHasher : IPasswordHasher
{
    public string Hash(string password, out string salt)
    {
        salt = "test-salt";
        return $"hash:{password}";
    }

    public bool Verify(string password, string hash, string salt)
        => hash == $"hash:{password}";
}
