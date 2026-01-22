using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

public interface IDiscountRepository
{
    Task<IReadOnlyCollection<Discount>> CreateBulk(IEnumerable<Discount> discounts,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Discount>> GetBy(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Discount>> Update(string code, IEnumerable<Discount> discounts,
        CancellationToken cancellationToken = default);

    Task<int> Delete(string code, CancellationToken cancellationToken = default);
}