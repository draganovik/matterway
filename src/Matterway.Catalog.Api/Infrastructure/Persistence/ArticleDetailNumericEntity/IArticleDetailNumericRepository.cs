using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;

public interface IArticleDetailNumericRepository
{
    Task<ArticleDetailNumeric?> GetBy(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailNumeric?> Create(ArticleDetailNumeric requestModel,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailNumeric?> Update(ArticleCode articleCode, string detailSlug, ArticleDetailNumeric request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default);
}