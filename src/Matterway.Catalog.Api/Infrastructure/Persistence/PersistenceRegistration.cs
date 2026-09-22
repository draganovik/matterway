using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
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
            builder.Services.AddScoped<IArticleDetailTextRepository, EfPgArticleDetailTextRepository>();
            builder.Services.AddScoped<IArticleDetailNumericRepository, EfPgArticleDetailNumericRepository>();
            builder.Services.AddScoped<IDetailRepository, EfPgDetailRepository>();
            builder.Services.AddScoped<IArticleImageRepository, EfPgArticleImageRepository>();
            builder.Services.AddScoped<IArticleRsqlRuleProvider, EfPgArticleRsqlRuleProvider>();
            builder.Services.AddScoped<IArticleRepository, EfPgArticleRepository>();

            return builder;
        }
    }
}