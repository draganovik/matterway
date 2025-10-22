using Customers.Api.Features.CartItems.Data;
using Customers.Api.Features.Customers.Data;
using Common.Infrastructure.ServiceBrokers;

namespace Customers.Api.Extensions;

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