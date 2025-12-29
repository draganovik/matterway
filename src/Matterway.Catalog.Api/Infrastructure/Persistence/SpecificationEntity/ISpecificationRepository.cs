using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;

public interface ISpecificationRepository
{
    Task<ICollection<Specification>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default);

    Task<Specification?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Specification?> UpsertAsync(Specification requestModel,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string slug, CancellationToken cancellationToken = default);
}