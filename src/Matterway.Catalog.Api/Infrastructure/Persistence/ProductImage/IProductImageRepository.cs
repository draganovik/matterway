using DomainProductImage = Matterway.Catalog.Api.Domain.Entities.ProductImage;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;

public interface IProductImageRepository
{
    Task<ICollection<DomainProductImage>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<DomainProductImage?> GetByOrderIndex(Guid parentId, int orderIndex,
        CancellationToken cancellationToken = default);

    Task<DomainProductImage?> GetById(Guid parentId, Guid id, CancellationToken cancellationToken = default);

    Task<DomainProductImage?> Create(DomainProductImage requestModel, CancellationToken cancellationToken = default);

    Task<DomainProductImage?> UpdateAsync(DomainProductImage request, int targetOrderIndex,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid parentId, int orderIndex, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(CancellationToken cancellationToken = default);
}