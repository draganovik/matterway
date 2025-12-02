using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Detail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Discount;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Product;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductSpecification;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Specification;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Application;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var postgresConnectionString = builder.Configuration.GetConnectionString("CatalogDb");
            if (string.IsNullOrEmpty(postgresConnectionString))
                throw new InvalidOperationException("Connection string 'CatalogDb' not found.");

            builder.Services.AddDbContext<CatalogDb>(options =>
                options.UseNpgsql(postgresConnectionString,
                    npgsqlOptions => { npgsqlOptions.EnableRetryOnFailure(); })
            );
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
            builder.Services.AddScoped<IDetailRepository, DetailRepository>();
            builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductSpecificationRepository, ProductSpecificationRepository>();
            builder.Services.AddScoped<ISpecificationRepository, SpecificationRepository>();

            return builder;
        }
    }
}