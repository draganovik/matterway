using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

public interface IDetailRepository
{
    Task<int> Count(string? titleLike, CancellationToken cancellationToken = default);

    Task<ICollection<Detail>> Query(int page, int pageSize, string? titleLike,
        CancellationToken cancellationToken = default);

    Task<Detail?> GetBy(string slug, CancellationToken cancellationToken = default);

    Task<Detail?> Upsert(Detail requestModel, CancellationToken cancellationToken = default);

    Task<bool> Delete(string slug, CancellationToken cancellationToken = default);
}