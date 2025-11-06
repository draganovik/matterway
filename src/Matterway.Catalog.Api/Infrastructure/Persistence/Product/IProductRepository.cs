using Matterway.Catalog.Api.Features.Products.Query;
using DomainProduct = Matterway.Catalog.Api.Domain.Product;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Product;

public interface IProductRepository
{
    Task<ICollection<DomainProduct>> Query(int pageIndex, int pageSize, QueryProductFilter queryProductFilter);

    Task<DomainProduct?> GetById(Guid id);

    Task<DomainProduct?> Create(DomainProduct requestModel);

    Task<DomainProduct?> UpdateAsync(DomainProduct entity);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities(QueryProductFilter queryProductFilter);
}