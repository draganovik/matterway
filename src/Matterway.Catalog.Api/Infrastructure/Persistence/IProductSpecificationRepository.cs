using DomainProductSpecification = Matterway.Catalog.Api.Domain.Entities.ProductSpecification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface IProductSpecificationRepository
{
    Task<ICollection<DomainProductSpecification>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<DomainProductSpecification?> GetByKey(Guid productId, string specificationSlug,
        CancellationToken cancellationToken = default);

    Task<DomainProductSpecification?> Create(DomainProductSpecification requestModel,
        CancellationToken cancellationToken = default);

    Task<DomainProductSpecification?> UpdateAsync(Guid productId, string specificationSlug,
        DomainProductSpecification request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid productId, string specificationSlug, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(CancellationToken cancellationToken = default);
}