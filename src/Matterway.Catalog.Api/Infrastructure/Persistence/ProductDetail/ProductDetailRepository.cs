using Microsoft.EntityFrameworkCore;
using DomainProductDetail = Matterway.Catalog.Api.Domain.ProductDetail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;

public sealed class ProductDetailRepository(CatalogDb context) : IProductDetailRepository
{
    public async Task<DomainProductDetail?> Create(DomainProductDetail requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ProductDetail.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ProductDetail.Include(x => x.Product).Include(x => x.Type)
                .FirstOrDefaultAsync(x => x.ProductId == requestModel.ProductId && x.TypeSlug == requestModel.TypeSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid productId, string typeSlug, CancellationToken cancellationToken = default)
    {
        var affected = await context.ProductDetail
            .Where(model => model.ProductId == productId && model.TypeSlug == typeSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<DomainProductDetail?> GetByKey(Guid productId, string typeSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductDetail
            .Include(pd => pd.Product)
            .Include(pd => pd.Type)
            .FirstOrDefaultAsync(pd => pd.ProductId == productId && pd.TypeSlug == typeSlug, cancellationToken);
    }

    public async Task<int> GetTotalEntities(CancellationToken cancellationToken = default)
    {
        return await context.ProductDetail.CountAsync(cancellationToken);
    }

    public async Task<ICollection<DomainProductDetail>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductDetail.Include(pd => pd.Product).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainProductDetail?> UpdateAsync(Guid productId, string typeSlug, DomainProductDetail request,
        CancellationToken cancellationToken = default)
    {
        context.ProductDetail.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ProductDetail
                .Include(x => x.Product)
                .Include(x => x.Type)
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.TypeSlug == typeSlug, cancellationToken);

        return null;
    }
}