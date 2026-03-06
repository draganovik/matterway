using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

public interface IArticleRepository
{
    Task<ICollection<Article>> Query(int pageIndex, int pageSize,
        string? filter, CancellationToken cancellationToken = default);

    Task<Article?> GetBy(ArticleCode code, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Article>> GetByCodes(IEnumerable<ArticleCode> codes,
        CancellationToken cancellationToken = default);

    Task<Article?> Create(Article requestModel, CancellationToken cancellationToken = default);

    Task<Article?> Update(Article entity, ArticleCode originalCode, CancellationToken cancellationToken = default);

    Task<bool> Delete(ArticleCode code, CancellationToken cancellationToken = default);

    Task<int> Count(string? filter, CancellationToken cancellationToken = default);
}