using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;

namespace Catalog.API.Repository;

public interface IProductImageRepository
{
    Task<ICollection<ProductImage>> Query(int pageIndex, int pageSize);

    Task<ProductImage?> GetById(Guid parentId, int id);

    Task<ProductImage?> Create(ProductImage requestModel);

    Task<ProductImage?> Update(Guid parentId, int id, ProductImageBaseRequestModel requestModel);

    Task<bool> Delete(Guid parentId, int id);
}
