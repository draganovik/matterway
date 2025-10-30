using Matterway.Common.Extensions;
using Matterway.Common.Services.Brokers;
using Matterway.Ordering.Api.Features.Addresses.Data;
using Matterway.Ordering.Api.Features.OrderHistories.Data;
using Matterway.Ordering.Api.Features.OrderItems.Data;
using Matterway.Ordering.Api.Features.Orders.Data;

namespace Matterway.Ordering.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddHttpClient<IIdentityServiceBroker, IdentityServiceBroker>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
        });

        services.AddHttpClient<ICatalogServiceBroker, CatalogServiceBroker>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = configuration.ResolveServiceUri("catalog-api", "Services:Catalog:Url");
        });

        services.AddHttpClient<ICustomersServiceBroker, CustomersServiceBroker>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = configuration.ResolveServiceUri("customers-api", "Services:Customers:Url");
        });

        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();

        return services;
    }
}