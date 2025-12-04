using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityDiscount;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProduct;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductSpecification;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntitySpecification;
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