using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Extensions;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
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
            builder.Services.AddScoped<IProductDetailTypeRepository, ProductDetailTypeRepository>();

            return builder;
        }
    }
}