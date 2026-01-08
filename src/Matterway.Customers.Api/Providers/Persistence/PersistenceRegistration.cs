using Matterway.Customers.Api.Providers.Persistence.AddressEntity;
using Matterway.Customers.Api.Providers.Persistence.CartItemEntity;
using Matterway.Customers.Api.Providers.Persistence.CustomerEntity;
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
            builder.Services.AddScoped<ICartItemRepository, EfPgCartItemRepository>();
            builder.Services.AddScoped<IAddressRepository, EfPgAddressRepository>();

            return builder;
        }
    }
}