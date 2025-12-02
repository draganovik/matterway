using Matterway.Catalog.Api.Domain;
using DomainDiscount = Matterway.Catalog.Api.Domain.Entities.Discount;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public interface IDiscountRepository
{
    Task<IReadOnlyCollection<DomainDiscount>> CreateManyAsync(IEnumerable<DomainDiscount> discounts,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DomainDiscount>> GetByCodeAsync(string code, ESupportedCurrency currency,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DomainDiscount>> ReplaceAsync(string code, ESupportedCurrency currency,
        IEnumerable<DomainDiscount> discounts, CancellationToken cancellationToken = default);

    Task<int> DeleteByCodeAsync(string code, CancellationToken cancellationToken = default);
}