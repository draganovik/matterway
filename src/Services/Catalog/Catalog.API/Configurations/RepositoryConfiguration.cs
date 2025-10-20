using Catalog.API.Repository;
using Shared.ServiceBrokers;

namespace Catalog.API.Configurations;

public static class RepositoryConfiguration
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();
        services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}