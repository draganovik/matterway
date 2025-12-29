using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImageEntity;

public interface IProductImageRepository
{
    Task<ICollection<ProductImage>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> GetByOrderIndex(Guid parentId, int orderIndex,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> GetById(Guid parentId, Guid id, CancellationToken cancellationToken = default);

    Task<ProductImage?> Create(ProductImage requestModel, CancellationToken cancellationToken = default);

    Task<ProductImage?> UpdateAsync(ProductImage request, int targetOrderIndex,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid parentId, int orderIndex, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(CancellationToken cancellationToken = default);
}