using Customers.API.Features.CartItems.Data;
using Customers.API.Features.Customers.Data;
using Shared.ServiceBrokers;

namespace Customers.API.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();
        services.AddScoped<ICatalogServiceBroker, CatalogServiceBroker>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();

        return services;
    }
}