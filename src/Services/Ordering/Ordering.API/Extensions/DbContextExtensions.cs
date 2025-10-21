using Microsoft.EntityFrameworkCore;
using Ordering.API.Data;

namespace Ordering.API.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("OrderingDbContext") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'OrderingDbContext' not found.")));

        return services;
    }
}