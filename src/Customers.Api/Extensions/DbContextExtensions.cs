using Customers.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDb>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CustomersDb") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'CustomersDb' not found.")));

        return services;
    }
}