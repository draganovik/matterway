using DomainProductDetail = Matterway.Catalog.Api.Domain.Entities.ProductDetail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;

public interface IProductDetailRepository
{
    Task<ICollection<DomainProductDetail>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<DomainProductDetail?> GetByKey(Guid productId, string detailSlug,
        CancellationToken cancellationToken = default);

    Task<DomainProductDetail?> Create(DomainProductDetail requestModel, CancellationToken cancellationToken = default);

    Task<DomainProductDetail?> UpdateAsync(Guid productId, string detailSlug, DomainProductDetail request,
        CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid productId, string detailSlug, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(CancellationToken cancellationToken = default);
}