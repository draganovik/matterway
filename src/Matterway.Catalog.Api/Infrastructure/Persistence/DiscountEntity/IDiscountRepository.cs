using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

public interface IDiscountRepository
{
    Task<IReadOnlyCollection<Discount>> CreateManyAsync(IEnumerable<Discount> discounts,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Discount>> GetByCodeAsync(string code, ESupportedCurrency currency,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Discount>> ReplaceAsync(string code, ESupportedCurrency currency,
        IEnumerable<Discount> discounts, CancellationToken cancellationToken = default);

    Task<int> DeleteByCodeAsync(string code, CancellationToken cancellationToken = default);
}