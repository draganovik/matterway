using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailEntity;

public sealed class EfPgProductDetailRepository(CatalogDb context) : IProductDetailRepository
{
    public async Task<ProductDetail?> Create(ProductDetail requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ProductDetail.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ProductDetail.Include(x => x.Product).Include(x => x.Detail)
                .FirstOrDefaultAsync(
                    x => x.ProductId == requestModel.ProductId && x.DetailSlug == requestModel.DetailSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid productId, string detailSlug, CancellationToken cancellationToken = default)
    {
        var affected = await context.ProductDetail
            .Where(model => model.ProductId == productId && model.DetailSlug == detailSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ProductDetail?> GetBy(Guid productId, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductDetail
            .Include(pd => pd.Product)
            .Include(pd => pd.Detail)
            .FirstOrDefaultAsync(pd => pd.ProductId == productId && pd.DetailSlug == detailSlug, cancellationToken);
    }

    public async Task<ICollection<ProductDetail>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductDetail.Include(pd => pd.Product).Include(pd => pd.Detail).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDetail?> Update(Guid productId, string detailSlug, ProductDetail request,
        CancellationToken cancellationToken = default)
    {
        context.ProductDetail.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ProductDetail
                .Include(x => x.Product)
                .Include(x => x.Detail)
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.DetailSlug == detailSlug, cancellationToken);

        return null;
    }
}