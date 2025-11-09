using DomainProductImage = Matterway.Catalog.Api.Domain.ProductImage;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;

public interface IProductImageRepository
{
    Task<ICollection<DomainProductImage>> Query(int pageIndex, int pageSize);

    Task<DomainProductImage?> GetByOrderIndex(Guid parentId, int orderIndex);

    Task<DomainProductImage?> GetById(Guid parentId, Guid id);

    Task<DomainProductImage?> Create(DomainProductImage requestModel);

    Task<DomainProductImage?> UpdateAsync(DomainProductImage request, int targetOrderIndex);

    Task<bool> Delete(Guid parentId, int orderIndex);

    Task<int> GetTotalEntities();
}