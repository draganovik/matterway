using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;

public interface IArticleSpecificationRepository
{
    Task<ArticleSpecification?> GetBy(Guid articleId, string specificationSlug,
        CancellationToken cancellationToken = default);

    Task<ArticleSpecification?> Create(ArticleSpecification requestModel,
        CancellationToken cancellationToken = default);

    Task<ArticleSpecification?> Update(Guid articleId, string specificationSlug,
        ArticleSpecification request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid articleId, string specificationSlug, CancellationToken cancellationToken = default);
}