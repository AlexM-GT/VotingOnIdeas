using VotingOnIdeas.Application.Interfaces;

namespace VotingOnIdeas.Infrastructure.Services;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password, out string salt)
    {
        salt = BCrypt.Net.BCrypt.GenerateSalt();
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    public bool Verify(string password, string hash, string salt)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
