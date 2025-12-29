using Matterway.Customers.Api.Infrastructure.Persistence;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCartItem;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCustomer;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Application;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("CustomersDb") ??
                                   throw new InvalidOperationException(
                                       "Connection string 'CustomersDb' not found.");

            builder.Services.AddDbContext<CustomersDb>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            builder.Services.AddScoped<ICustomerRepository, EfPgCustomerRepository>();
            builder.Services.AddScoped<ICartItemRepository, EfPgCartItemRepository>();

            return builder;
        }
    }
}