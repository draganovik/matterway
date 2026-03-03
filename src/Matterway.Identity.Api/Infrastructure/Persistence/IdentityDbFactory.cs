using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Matterway.Identity.Api.Infrastructure.Persistence;

public sealed class IdentityDbFactory : IDesignTimeDbContextFactory<IdentityDbComposer>
{
    public IdentityDbComposer CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("Properties/appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("IdentityDb")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'IdentityDb' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbComposer>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());

        return new IdentityDbComposer(optionsBuilder.Options);
    }
}