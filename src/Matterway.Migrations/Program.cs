using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Customers.Api.Infrastructure.Persistence;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Matterway.Sales.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

ConfigureDbContext<CatalogDbComposer>(builder, "CatalogDb");
ConfigureDbContext<CustomersDbComposer>(builder, "CustomersDb");
ConfigureDbContext<IdentityDbComposer>(builder, "IdentityDb");
ConfigureDbContext<SalesDbComposer>(builder, "SalesDb");

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var logger = scope.ServiceProvider
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("Matterway.Migrations");

await MigrateAsync<IdentityDbComposer>(scope.ServiceProvider, logger);
await MigrateAsync<CatalogDbComposer>(scope.ServiceProvider, logger);
await MigrateAsync<CustomersDbComposer>(scope.ServiceProvider, logger);
await MigrateAsync<SalesDbComposer>(scope.ServiceProvider, logger);

logger.LogInformation("All database migrations completed successfully.");

static void ConfigureDbContext<TContext>(IHostApplicationBuilder builder, string connectionName)
    where TContext : DbContext
{
    var connectionString = builder.Configuration.GetConnectionString(connectionName)
                           ?? throw new InvalidOperationException($"Connection string '{connectionName}' not found.");

    builder.Services.AddDbContext<TContext>(options =>
        options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));
}

static async Task MigrateAsync<TContext>(IServiceProvider services, ILogger logger)
    where TContext : DbContext
{
    var contextName = typeof(TContext).Name;
    logger.LogInformation("Applying migrations for {DbContext}...", contextName);

    var dbContext = services.GetRequiredService<TContext>();
    await dbContext.Database.MigrateAsync();

    logger.LogInformation("Migrations applied for {DbContext}.", contextName);
}