using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityDiscount;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityProduct;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityProductSpecification;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntitySpecification;
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