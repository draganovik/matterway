using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;

public interface IArticleDetailTextRepository
{
    Task<ArticleDetailText?> GetBy(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ArticleDetailText?> Create(ArticleDetailText requestModel, CancellationToken cancellationToken = default);

    Task<ArticleDetailText?> Update(ArticleCode articleCode, string detailSlug, ArticleDetailText request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default);
}