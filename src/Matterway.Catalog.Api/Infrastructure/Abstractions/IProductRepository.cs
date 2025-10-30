using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.Products.Query;

namespace Matterway.Catalog.Api.Infrastructure.Abstractions;

public interface IProductRepository
{
    Task<ICollection<Product>> Query(int pageIndex, int pageSize, QueryProductFilter queryProductFilter);

    Task<Product?> GetById(Guid id);

    Task<Product?> Create(Product requestModel);

    Task<Product?> UpdateAsync(Product entity);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities(QueryProductFilter queryProductFilter);
}