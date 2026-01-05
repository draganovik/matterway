using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

public sealed class EfPgDiscountRepository(CatalogDb context) : IDiscountRepository
{
    public async Task<IReadOnlyCollection<Discount>> CreateBulk(IEnumerable<Discount> discounts,
        CancellationToken cancellationToken = default)
    {
        var discountList = discounts.ToList();
        if (discountList.Count == 0) return [];

        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await context.Discount.AddRangeAsync(discountList, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return discountList;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<IReadOnlyCollection<Discount>> GetBy(string code, ESupportedCurrency currency,
        CancellationToken cancellationToken = default)
    {
        return await context.Discount
            .Include(d => d.Price)
            .Where(d => d.Code == code && d.Currency == currency)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Discount>> Update(string code, ESupportedCurrency currency,
        IEnumerable<Discount> discounts, CancellationToken cancellationToken = default)
    {
        var discountList = discounts.ToList();
        if (discountList.Count == 0) return [];

        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await context.Discount
                    .Where(d => d.Code == code && d.Currency == currency)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.Discount.AddRangeAsync(discountList, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return discountList;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<int> Delete(string code, CancellationToken cancellationToken = default)
    {
        return await context.Discount
            .Where(d => d.Code == code)
            .ExecuteDeleteAsync(cancellationToken);
    }
}