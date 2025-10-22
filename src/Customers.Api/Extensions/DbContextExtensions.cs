using Customers.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CustomersDbContext") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'CustomersDbContext' not found.")));

        return services;
    }
}