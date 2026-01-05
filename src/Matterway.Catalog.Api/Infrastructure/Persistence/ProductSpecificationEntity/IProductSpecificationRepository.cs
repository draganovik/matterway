using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductSpecificationEntity;

public interface IProductSpecificationRepository
{
    Task<ProductSpecification?> GetBy(Guid productId, string specificationSlug,
        CancellationToken cancellationToken = default);

    Task<ProductSpecification?> Create(ProductSpecification requestModel,
        CancellationToken cancellationToken = default);

    Task<ProductSpecification?> Update(Guid productId, string specificationSlug,
        ProductSpecification request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid productId, string specificationSlug, CancellationToken cancellationToken = default);
}