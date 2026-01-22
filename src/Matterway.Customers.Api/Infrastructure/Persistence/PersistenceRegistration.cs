using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence;

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