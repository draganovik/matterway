using Catalog.API.Features.ProductImages.Contracts;
using Catalog.API.Features.ProductImages.Domain;

namespace Catalog.API.Features.ProductImages.Data;

public interface IProductImageRepository
{
    Task<ICollection<ProductImage>> Query(int pageIndex, int pageSize);

    Task<ProductImage?> GetById(Guid parentId, int id);

    Task<ProductImage?> Create(ProductImage requestModel);

    Task<ProductImage?> Update(Guid parentId, int id, ProductImageBaseRequest request);

    Task<bool> Delete(Guid parentId, int id);

    Task<int> GetTotalEntities();
}