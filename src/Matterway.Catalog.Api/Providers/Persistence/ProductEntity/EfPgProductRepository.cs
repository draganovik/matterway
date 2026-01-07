using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductEntity;

public sealed class EfPgProductRepository(CatalogDb context)
    : IProductRepository
{
    public async Task<Product?> Create(Product requestModel, CancellationToken cancellationToken = default)
    {
        context.Product.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.Product
                .Include(x => x.Prices)
                .Include(x => x.ProductDetails!)
                .ThenInclude(pd => pd!.Detail)
                .Include(x => x.ProductSpecifications!)
                .ThenInclude(ps => ps!.Specification)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id, cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var exists = await context.Product.AnyAsync(p => p.Id == id, cancellationToken);
                if (!exists)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                await context.Price
                    .Where(model => model.ProductId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ProductImage
                    .Where(model => model.ProductId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ProductDetail
                    .Where(model => model.ProductId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ProductSpecification
                    .Where(model => model.ProductId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                var affected = await context.Product
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync(cancellationToken);

                if (affected != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<Product?> GetBy(Guid id, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.Product
            .Include(x => x.Prices)
            .ThenInclude(p => p.Discounts.Where(d => d.ValidFrom <= now && (d.ValidTo == null || d.ValidTo >= now)))
            .Include(x => x.ProductDetails!)
            .ThenInclude(pd => pd!.Detail)
            .Include(x => x.ProductSpecifications!)
            .ThenInclude(ps => ps!.Specification)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> Count(string? filter,
        CancellationToken cancellationToken = default)
    {
        var productQuery = context.Product.AsQueryable().ApplyProductRsql(filter);
        return await productQuery.CountAsync(cancellationToken);
    }

    public async Task<ICollection<Product>> Query(
        int pageIndex,
        int pageSize,
        string? filter,
        CancellationToken cancellationToken = default)
    {
        var productQuery = context.Product.AsQueryable().ApplyProductRsql(filter);
        var now = DateTime.UtcNow;
        return await productQuery
            .Include(x => x.Prices)
            .ThenInclude(p => p.Discounts.Where(d => d.ValidFrom <= now && (d.ValidTo == null || d.ValidTo >= now)))
            .AsNoTracking()
            .Include(x => x.ProductImages).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> Update(Product request, CancellationToken cancellationToken = default)
    {
        context.Product.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.Product
                .Include(x => x.ProductDetails!)
                .ThenInclude(pd => pd.Detail)
                .Include(x => x.ProductSpecifications)
                .ThenInclude(ps => ps.Specification)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return null;
    }
}