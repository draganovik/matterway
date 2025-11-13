using Matterway.Customers.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Extensions;

public static class PersistenceRegistration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureDatabase(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CustomersDb") ??
                                   throw new InvalidOperationException(
                                       "Connection string 'CustomersDb' not found.");

            services.AddDbContext<CustomersDb>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            return services;
        }
    }
}