using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

public interface IArticleRepository
{
    Task<ICollection<Article>> Query(int pageIndex, int pageSize,
        string? filter, CancellationToken cancellationToken = default);

    Task<Article?> GetBy(Guid id, CancellationToken cancellationToken = default);

    Task<Article?> Create(Article requestModel, CancellationToken cancellationToken = default);

    Task<Article?> Update(Article entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<int> Count(string? filter, CancellationToken cancellationToken = default);
}