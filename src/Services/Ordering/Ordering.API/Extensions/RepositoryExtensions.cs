using Ordering.API.Features.Addresses.Data;
using Ordering.API.Features.OrderHistories.Data;
using Ordering.API.Features.OrderItems.Data;
using Ordering.API.Features.Orders.Data;
using Shared.ServiceBrokers;

namespace Ordering.API.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();
        services.AddScoped<ICatalogServiceBroker, CatalogServiceBroker>();
        services.AddScoped<ICustomersServiceBroker, CustomersServiceBroker>();

        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();

        return services;
    }
}