using DomainProduct = Matterway.Catalog.Api.Domain.Entities.Product;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface IProductRepository
{
    Task<ICollection<DomainProduct>> Query(int pageIndex, int pageSize,
        string? filter, CancellationToken cancellationToken = default);

    Task<DomainProduct?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<DomainProduct?> Create(DomainProduct requestModel, CancellationToken cancellationToken = default);

    Task<DomainProduct?> UpdateAsync(DomainProduct entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<int> GetTotalEntities(string? filter, CancellationToken cancellationToken = default);
}