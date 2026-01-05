using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductEntity;

public interface IProductRepository
{
    Task<ICollection<Product>> Query(int pageIndex, int pageSize,
        string? filter, CancellationToken cancellationToken = default);

    Task<Product?> GetBy(Guid id, CancellationToken cancellationToken = default);

    Task<Product?> Create(Product requestModel, CancellationToken cancellationToken = default);

    Task<Product?> Update(Product entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<int> Count(string? filter, CancellationToken cancellationToken = default);
}