using Microsoft.EntityFrameworkCore;
using Payments.Api.Data;

namespace Payments.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDb>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PaymentsDb") ??
                                 throw new InvalidOperationException(
                                     "Connection string 'PaymentsDb' not found.")));

        return services;
    }
}