using Microsoft.EntityFrameworkCore;
using Payments.Api.Data;

namespace Payments.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PaymentsDbContext") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'PaymentsDbContext' not found.")));

        return services;
    }
}