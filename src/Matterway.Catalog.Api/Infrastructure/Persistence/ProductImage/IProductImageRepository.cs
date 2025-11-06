using DomainProductImage = Matterway.Catalog.Api.Domain.ProductImage;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;

public interface IProductImageRepository
{
    Task<ICollection<DomainProductImage>> Query(int pageIndex, int pageSize);

    Task<DomainProductImage?> GetById(Guid parentId, int id);

    Task<DomainProductImage?> Create(DomainProductImage requestModel);

    Task<DomainProductImage?> UpdateAsync(DomainProductImage request);

    Task<bool> Delete(Guid parentId, int id);

    Task<int> GetTotalEntities();
}