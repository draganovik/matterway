using Catalog.API.Features.Products.Contracts;
using Catalog.API.Features.Products.Domain;
using Catalog.API.Features.Products.Shared;

namespace Catalog.API.Features.Products.Data;

public interface IProductRepository
{
    Task<ICollection<Product>> Query(int pageIndex, int pageSize, ProductFilter productFilter);

    Task<Product?> GetById(Guid id);

    Task<Product?> Create(Product requestModel);

    Task<Product?> Update(Guid id, ProductBaseRequest request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities(ProductFilter productFilter);
}