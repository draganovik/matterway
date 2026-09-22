using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

internal interface IArticleRsqlRuleProvider
{
    Task<ArticleRsqlRuleSet> GetAsync(CancellationToken cancellationToken = default);
}