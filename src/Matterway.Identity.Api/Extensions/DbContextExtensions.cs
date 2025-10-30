using Matterway.Identity.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDb>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityDb")
                                 ?? throw new InvalidOperationException(
                                     "Connection string 'IdentityDb' not found.")));

        return services;
    }
}