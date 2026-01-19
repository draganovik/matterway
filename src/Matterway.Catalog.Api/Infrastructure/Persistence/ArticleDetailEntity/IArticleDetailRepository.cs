using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;

public interface IArticleDetailRepository
{
    Task<ArticleDetail?> GetBy(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ArticleDetail?> Create(ArticleDetail requestModel, CancellationToken cancellationToken = default);

    Task<ArticleDetail?> Update(Guid articleId, string detailSlug, ArticleDetail request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid articleId, string detailSlug, CancellationToken cancellationToken = default);
}