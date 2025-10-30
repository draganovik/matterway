using Matterway.Ordering.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Ordering.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderingDb>(options =>
            options.UseSqlServer(configuration.GetConnectionString("OrderingDb") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'OrderingDb' not found.")));

        return services;
    }
}