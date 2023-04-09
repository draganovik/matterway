using Catalog.API.Entities;
using Catalog.API.Models.ProductModels;

namespace Catalog.API.Repository;

public interface IProductRepository
{
    Task<ICollection<Product>> Query();

    Task<Product?> GetById(Guid id);

    Task<Product?> Create(Product requestModel);

    Task<Product?> Update(Guid id, ProductBaseRequestModel requestModel);

    Task<bool> Delete(Guid id);
}
