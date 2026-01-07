using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductDetailEntity;

public interface IProductDetailRepository
{
    Task<ProductDetail?> GetBy(Guid productId, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ProductDetail?> Create(ProductDetail requestModel, CancellationToken cancellationToken = default);

    Task<ProductDetail?> Update(Guid productId, string detailSlug, ProductDetail request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid productId, string detailSlug, CancellationToken cancellationToken = default);
}