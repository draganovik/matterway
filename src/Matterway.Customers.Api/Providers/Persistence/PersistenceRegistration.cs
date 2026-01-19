using Matterway.Customers.Api.Providers.Persistence.AddressEntity;
using Matterway.Customers.Api.Providers.Persistence.CustomerArticleEntity;
using Matterway.Customers.Api.Providers.Persistence.CustomerEntity;
using Matterway.Customers.Api.Providers.Persistence.CustomerOrderEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Providers.Persistence;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("CustomersDb") ??
                                   throw new InvalidOperationException(
                                       "Connection string 'CustomersDb' not found.");

            builder.Services.AddDbContext<CustomersDbComposer>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            builder.Services.AddScoped<ICustomerRepository, EfPgCustomerRepository>();
            builder.Services.AddScoped<ICustomerArticleRepository, EfPgCustomerArticleRepository>();
            builder.Services.AddScoped<ICustomerOrderRepository, EfPgCustomerOrderRepository>();
            builder.Services.AddScoped<IAddressRepository, EfPgAddressRepository>();

            return builder;
        }
    }
}