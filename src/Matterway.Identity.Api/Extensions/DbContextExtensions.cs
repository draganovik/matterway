using Matterway.Identity.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'IdentityDb' not found.");

        services.AddDbContext<IdentityDb>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

        return services;
    }
}