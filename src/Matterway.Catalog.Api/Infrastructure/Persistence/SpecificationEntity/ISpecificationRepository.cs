using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;

public interface ISpecificationRepository
{
    Task<ICollection<Specification>> Query(string? titleLike, int limit,
        CancellationToken cancellationToken = default);

    Task<Specification?> GetBy(string slug, CancellationToken cancellationToken = default);

    Task<Specification?> Upsert(Specification requestModel,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(string slug, CancellationToken cancellationToken = default);
}