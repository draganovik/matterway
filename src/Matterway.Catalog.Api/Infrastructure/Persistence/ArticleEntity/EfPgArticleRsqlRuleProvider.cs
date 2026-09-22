using Matterway.Catalog.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

internal sealed class EfPgArticleRsqlRuleProvider(CatalogDbComposer context) : IArticleRsqlRuleProvider
{
    private ArticleRsqlRuleSet? _rules;

    public async Task<ArticleRsqlRuleSet> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_rules is not null) return _rules;

        var details = await context.Detail
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        _rules = ArticleRsqlRuleSet.FromDetails(details);
        return _rules;
    }
}