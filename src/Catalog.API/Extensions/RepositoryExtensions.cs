using Catalog.API.Features.ProductDetails.Data;
using Catalog.API.Features.ProductImages.Data;
using Catalog.API.Features.Products.Data;
using Shared.ServiceBrokers;

namespace Catalog.API.Extensions;

public static class RepositoryExtensions
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