using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Infrastructure.Abstractions;

public interface IProductImageRepository
{
    Task<ICollection<ProductImage>> Query(int pageIndex, int pageSize);

    Task<ProductImage?> GetById(Guid parentId, int id);

    Task<ProductImage?> Create(ProductImage requestModel);

    Task<ProductImage?> UpdateAsync(ProductImage request);

    Task<bool> Delete(Guid parentId, int id);

    Task<int> GetTotalEntities();
}