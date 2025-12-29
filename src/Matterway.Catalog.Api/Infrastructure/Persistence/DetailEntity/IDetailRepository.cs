using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

public interface IDetailRepository
{
    Task<ICollection<Detail>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default);

    Task<Detail?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Detail?> UpsertAsync(Detail requestModel, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string slug, CancellationToken cancellationToken = default);
}