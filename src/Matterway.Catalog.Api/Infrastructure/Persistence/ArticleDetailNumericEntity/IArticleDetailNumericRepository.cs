using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;

public interface IArticleDetailNumericRepository
{
    Task<ArticleDetailNumeric?> GetBy(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailNumeric?> Create(ArticleDetailNumeric requestModel,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailNumeric?> Update(Guid articleId, string detailSlug, ArticleDetailNumeric request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default);
}