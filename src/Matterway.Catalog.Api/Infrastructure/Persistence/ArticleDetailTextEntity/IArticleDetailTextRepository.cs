using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;

public interface IArticleDetailTextRepository
{
    Task<ArticleDetailText?> GetBy(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailText?> Create(ArticleDetailText requestModel, CancellationToken cancellationToken = default);

    Task<ArticleDetailText?> Update(Guid articleId, string detailSlug, ArticleDetailText request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default);
}