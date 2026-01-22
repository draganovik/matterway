using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderStatusEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Sales.Api.Infrastructure.Persistence;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("SalesDb") ??
                                   throw new InvalidOperationException(
                                       "Connection string 'SalesDb' not found.");

            builder.Services.AddDbContext<SalesDbComposer>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

            builder.Services.AddScoped<IOrderRepository, EfPgOrderRepository>();
            builder.Services.AddScoped<IOrderStatusRepository, EfPgOrderStatusRepository>();
            builder.Services.AddScoped<IPaymentRepository, EfPgPaymentRepository>();

            return builder;
        }
    }
}