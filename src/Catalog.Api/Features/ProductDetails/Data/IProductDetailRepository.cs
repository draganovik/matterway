using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Domain;

namespace Catalog.Api.Features.ProductDetails.Data;

public interface IProductDetailRepository
{
    Task<ICollection<ProductDetail>> Query(int pageIndex, int pageSize);

    Task<ProductDetail?> GetById(Guid id);

    Task<ProductDetail?> Create(ProductDetail requestModel);

    Task<ProductDetail?> Update(Guid id, ProductDetailBaseRequest request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}