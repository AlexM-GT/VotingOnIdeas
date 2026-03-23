using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VotingOnIdeas.Infrastructure.Persistence;

namespace VotingOnIdeas.API.Tests.Infrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Register an InMemory AppDbContext that overrides the SQL Server one.
            // Using a factory delegate bypasses EF's IDbContextOptionsConfiguration pipeline
            // which would otherwise merge SqlServer + InMemory extensions and throw.
            services.AddScoped(_ =>
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(_dbName)
                    .Options;
                return new AppDbContext(options);
            });
        });
    }
}
