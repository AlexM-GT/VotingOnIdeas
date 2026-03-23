using Microsoft.EntityFrameworkCore;
using VotingOnIdeas.Application.Interfaces;
using VotingOnIdeas.Domain.Constants;
using VotingOnIdeas.Domain.Entities;
using VotingOnIdeas.Infrastructure.Persistence;

namespace VotingOnIdeas.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var hash = passwordHasher.Hash("Admin123", out var salt);
            var admin = User.Create("admin", "admin@votingonideas.local", hash, salt);
            admin.AssignRole(UserRole.Admin);

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
