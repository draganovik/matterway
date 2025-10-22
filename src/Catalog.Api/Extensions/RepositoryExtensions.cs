using Catalog.Api.Features.ProductDetails.Data;
using Catalog.Api.Features.ProductImages.Data;
using Catalog.Api.Features.Products.Data;
using Common.Infrastructure.Services.Brokers;

namespace Catalog.Api.Extensions;

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