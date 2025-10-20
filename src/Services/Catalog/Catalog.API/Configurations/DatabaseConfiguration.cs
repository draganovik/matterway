using Microsoft.EntityFrameworkCore;
using Catalog.API.Data;


namespace Catalog.API.Configurations;

public static class DatabaseConfiguration
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