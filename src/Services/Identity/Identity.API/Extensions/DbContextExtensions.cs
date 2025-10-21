using Identity.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityDbContext")
                                 ?? throw new InvalidOperationException(
                                     "Connection string 'IdentityDbContext' not found.")));

        return services;
    }
}