using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

public interface IArticleImageRepository
{
    Task<ArticleImage?> GetBy(ArticleCode articleCode, int orderIndex,
        CancellationToken cancellationToken = default);

    Task<ArticleImage?> Create(ArticleImage requestModel, CancellationToken cancellationToken = default);

    Task<ArticleImage?> Update(ArticleImage request, int targetOrderIndex,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(ArticleCode articleCode, int orderIndex, CancellationToken cancellationToken = default);
}