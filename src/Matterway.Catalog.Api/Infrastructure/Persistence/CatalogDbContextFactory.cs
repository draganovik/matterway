using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDb>
{
    public CatalogDb CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Properties/appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CatalogDb")
                               ?? throw new InvalidOperationException("Connection string 'CatalogDb' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<CatalogDb>();
        optionsBuilder.UseNpgsql(connectionString,
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());

        return new CatalogDb(optionsBuilder.Options);
    }
}
