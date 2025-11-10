using Matterway.Catalog.Api.Features.Products;
using DomainProduct = Matterway.Catalog.Api.Domain.Product;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Product;

public interface IProductRepository
{
    Task<ICollection<DomainProduct>> Query(int pageIndex, int pageSize,
        QueryProducts.QueryProductFilter queryProductFilter, CancellationToken cancellationToken = default);

    Task<DomainProduct?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<DomainProduct?> Create(DomainProduct requestModel, CancellationToken cancellationToken = default);

    Task<DomainProduct?> UpdateAsync(DomainProduct entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(QueryProducts.QueryProductFilter queryProductFilter,
        CancellationToken cancellationToken = default);
}