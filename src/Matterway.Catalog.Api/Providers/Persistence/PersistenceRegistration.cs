using Matterway.Catalog.Api.Providers.Persistence.DetailEntity;
using Matterway.Catalog.Api.Providers.Persistence.DiscountEntity;
using Matterway.Catalog.Api.Providers.Persistence.ProductDetailEntity;
using Matterway.Catalog.Api.Providers.Persistence.ProductEntity;
using Matterway.Catalog.Api.Providers.Persistence.ProductImageEntity;
using Matterway.Catalog.Api.Providers.Persistence.ProductSpecificationEntity;
using Matterway.Catalog.Api.Providers.Persistence.SpecificationEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Providers.Persistence;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var postgresConnectionString = builder.Configuration.GetConnectionString("CatalogDb") ??
                                           throw new InvalidOperationException(
                                               "Connection string 'CatalogDb' not found.");

            builder.Services.AddDbContext<CatalogDb>(options =>
                options.UseNpgsql(postgresConnectionString,
                    npgsqlOptions => { npgsqlOptions.EnableRetryOnFailure(); })
            );
            builder.Services.AddScoped<IDiscountRepository, EfPgDiscountRepository>();
            builder.Services.AddScoped<IProductDetailRepository, EfPgProductDetailRepository>();
            builder.Services.AddScoped<IDetailRepository, EfPgDetailRepository>();
            builder.Services.AddScoped<IProductImageRepository, EfPgProductImageRepository>();
            builder.Services.AddScoped<IProductRepository, EfPgProductRepository>();
            builder.Services.AddScoped<IProductSpecificationRepository, EfPgProductSpecificationRepository>();
            builder.Services.AddScoped<ISpecificationRepository, EfPgSpecificationRepository>();

            return builder;
        }
    }
}