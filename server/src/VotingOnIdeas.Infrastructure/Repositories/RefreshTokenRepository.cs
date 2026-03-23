using Microsoft.EntityFrameworkCore;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;
using VotingOnIdeas.Infrastructure.Persistence;

namespace VotingOnIdeas.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : Repository<RefreshToken, Guid>, IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetActiveTokenAsync(string token, CancellationToken cancellationToken = default)
        => await _context.RefreshTokens
            .FirstOrDefaultAsync(
                rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var t in tokens)
            t.Revoke();
    }
}
