using DomainDetail = Matterway.Catalog.Api.Domain.Entities.Detail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface IDetailRepository
{
    Task<ICollection<DomainDetail>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default);

    Task<DomainDetail?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}