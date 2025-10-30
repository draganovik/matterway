using Catalog.Api.Domain;
using Catalog.Api.Features.ProductImages.Contracts;

namespace Catalog.Api.Infrastructure.Abstractions;

public interface IProductImageRepository
{
    Task<ICollection<ProductImage>> Query(int pageIndex, int pageSize);

    Task<ProductImage?> GetById(Guid parentId, int id);

    Task<ProductImage?> Create(ProductImage requestModel);

    Task<ProductImage?> Update(Guid parentId, int id, ProductImageBaseRequest request);

    Task<bool> Delete(Guid parentId, int id);

    Task<int> GetTotalEntities();
}