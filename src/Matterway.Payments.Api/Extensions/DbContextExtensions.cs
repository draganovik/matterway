using Matterway.Payments.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Payments.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentsDb") ??
                               throw new InvalidOperationException(
                                   "Connection string 'PaymentsDb' not found.");

        services.AddDbContext<PaymentsDb>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

        return services;
    }
}