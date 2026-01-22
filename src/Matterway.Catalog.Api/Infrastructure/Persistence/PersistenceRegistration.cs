using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var postgresConnectionString = builder.Configuration.GetConnectionString("CatalogDb") ??
                                           throw new InvalidOperationException(
                                               "Connection string 'CatalogDb' not found.");

            builder.Services.AddDbContext<CatalogDbComposer>(options =>
                options.UseNpgsql(postgresConnectionString,
                    npgsqlOptions => { npgsqlOptions.EnableRetryOnFailure(); })
            );
            builder.Services.AddScoped<IDiscountRepository, EfPgDiscountRepository>();
            builder.Services.AddScoped<IArticleDetailRepository, EfPgArticleDetailRepository>();
            builder.Services.AddScoped<IDetailRepository, EfPgDetailRepository>();
            builder.Services.AddScoped<IArticleImageRepository, EfPgArticleImageRepository>();
            builder.Services.AddScoped<IArticleRepository, EfPgArticleRepository>();
            builder.Services.AddScoped<IArticleSpecificationRepository, EfPgArticleSpecificationRepository>();
            builder.Services.AddScoped<ISpecificationRepository, EfPgSpecificationRepository>();

            return builder;
        }
    }
}