using DomainSpecification = Matterway.Catalog.Api.Domain.Entities.Specification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface ISpecificationRepository
{
    Task<ICollection<DomainSpecification>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default);

    Task<DomainSpecification?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}