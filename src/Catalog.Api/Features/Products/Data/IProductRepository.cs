using Catalog.Api.Domain;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Shared;

namespace Catalog.Api.Features.Products.Data;

public interface IProductRepository
{
    Task<ICollection<Product>> Query(int pageIndex, int pageSize, ProductFilter productFilter);

    Task<Product?> GetById(Guid id);

    Task<Product?> Create(Product requestModel);

    Task<Product?> Update(Guid id, ProductBaseRequest request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities(ProductFilter productFilter);
}