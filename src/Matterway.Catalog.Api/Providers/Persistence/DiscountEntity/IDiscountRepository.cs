using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Providers.Persistence.DiscountEntity;

public interface IDiscountRepository
{
    Task<IReadOnlyCollection<Discount>> CreateBulk(IEnumerable<Discount> discounts,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Discount>> GetBy(string code, ESupportedCurrency currency,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Discount>> Update(string code, ESupportedCurrency currency,
        IEnumerable<Discount> discounts, CancellationToken cancellationToken = default);

    Task<int> Delete(string code, CancellationToken cancellationToken = default);
}