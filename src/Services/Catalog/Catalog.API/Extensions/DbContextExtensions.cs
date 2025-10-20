using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CatalogDbContext")
                                 ?? throw new InvalidOperationException(
                                     "Connection string 'CatalogDbContext' not found.")));

        return services;
    }
}