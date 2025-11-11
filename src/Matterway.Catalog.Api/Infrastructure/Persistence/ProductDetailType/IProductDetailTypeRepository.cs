using DomainProductDetailType = Matterway.Catalog.Api.Domain.ProductDetailType;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;

public interface IProductDetailTypeRepository
{
    Task<ICollection<DomainProductDetailType>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default);

    Task<DomainProductDetailType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}