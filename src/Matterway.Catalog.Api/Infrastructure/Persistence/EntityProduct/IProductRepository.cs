using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.EntityProduct;

public interface IProductRepository
{
    Task<ICollection<Product>> Query(int pageIndex, int pageSize,
        string? filter, CancellationToken cancellationToken = default);

    Task<Product?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<Product?> Create(Product requestModel, CancellationToken cancellationToken = default);

    Task<Product?> UpdateAsync(Product entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(string? filter, CancellationToken cancellationToken = default);
}