using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Extensions;

public static class PersistenceExtensions
{
    public static void ConfigurePersistence(this IHostApplicationBuilder builder)
    {
        var postgresConnectionString = builder.Configuration.GetConnectionString("CatalogDb");

        if (string.IsNullOrEmpty(postgresConnectionString))
        {
            throw new InvalidOperationException("Connection string 'CatalogDb' not found.");
        }

        builder.Services.AddDbContext<CatalogDb>(options =>
            options.UseNpgsql(postgresConnectionString,
                npgsqlOptions => { npgsqlOptions.EnableRetryOnFailure(); })
        );

        builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
        builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
    }
}