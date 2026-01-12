using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductImageEntity;

public interface IProductImageRepository
{
    Task<ProductImage?> GetBy(Guid parentId, int orderIndex,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> Create(ProductImage requestModel, CancellationToken cancellationToken = default);

    Task<ProductImage?> Update(ProductImage request, int targetOrderIndex,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid parentId, int orderIndex, CancellationToken cancellationToken = default);
}