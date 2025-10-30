using Matterway.Common.Extensions;
using Matterway.Common.Services.Brokers;
using Matterway.Customers.Api.Features.CartItems.Data;
using Matterway.Customers.Api.Features.Customers.Data;

namespace Matterway.Customers.Api.Extensions;

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

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();

        return services;
    }
}