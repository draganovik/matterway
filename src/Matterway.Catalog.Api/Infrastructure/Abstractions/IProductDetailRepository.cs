using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.ProductDetails.Contracts;

namespace Matterway.Catalog.Api.Infrastructure.Abstractions;

public interface IProductDetailRepository
{
    Task<ICollection<ProductDetail>> Query(int pageIndex, int pageSize);

    Task<ProductDetail?> GetById(Guid id);

    Task<ProductDetail?> Create(ProductDetail requestModel);

    Task<ProductDetail?> Update(Guid id, ProductDetailBaseRequest request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}