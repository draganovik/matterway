using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Infrastructure.Abstractions;

public interface IProductDetailRepository
{
    Task<ICollection<ProductDetail>> Query(int pageIndex, int pageSize);

    Task<ProductDetail?> GetById(Guid id);

    Task<ProductDetail?> Create(ProductDetail requestModel);

    Task<ProductDetail?> UpdateAsync(ProductDetail request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}