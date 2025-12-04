using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface IProductSpecificationRepository
{
    Task<ICollection<ProductSpecification>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductSpecification?> GetByKey(Guid productId, string specificationSlug,
        CancellationToken cancellationToken = default);

    Task<ProductSpecification?> Create(ProductSpecification requestModel,
        CancellationToken cancellationToken = default);

    Task<ProductSpecification?> UpdateAsync(Guid productId, string specificationSlug,
        ProductSpecification request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid productId, string specificationSlug, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(CancellationToken cancellationToken = default);
}