using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface IProductDetailRepository
{
    Task<ICollection<ProductDetail>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductDetail?> GetByKey(Guid productId, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<ProductDetail?> Create(ProductDetail requestModel, CancellationToken cancellationToken = default);

    Task<ProductDetail?> UpdateAsync(Guid productId, string detailSlug, ProductDetail request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid productId, string detailSlug, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(CancellationToken cancellationToken = default);
}