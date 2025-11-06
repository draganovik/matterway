using DomainProductDetail = Matterway.Catalog.Api.Domain.ProductDetail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;

public interface IProductDetailRepository
{
    Task<ICollection<DomainProductDetail>> Query(int pageIndex, int pageSize);

    Task<DomainProductDetail?> GetById(Guid id);

    Task<DomainProductDetail?> Create(DomainProductDetail requestModel);

    Task<DomainProductDetail?> UpdateAsync(DomainProductDetail request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}