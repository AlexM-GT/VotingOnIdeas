using Microsoft.EntityFrameworkCore;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Domain.Interfaces;
using VotingOnIdeas.Infrastructure.Persistence;

namespace VotingOnIdeas.Infrastructure.Repositories;

public sealed class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => DbSet.AnyAsync(u => u.Username == username, cancellationToken);
}
