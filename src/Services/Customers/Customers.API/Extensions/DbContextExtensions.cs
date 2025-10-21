using Customers.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Extensions;

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