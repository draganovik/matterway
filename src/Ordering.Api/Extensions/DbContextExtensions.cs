using Microsoft.EntityFrameworkCore;
using Ordering.Api.Data;

namespace Ordering.Api.Extensions;

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