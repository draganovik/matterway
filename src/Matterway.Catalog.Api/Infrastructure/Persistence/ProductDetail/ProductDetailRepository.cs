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
        {
            return await context.ProductDetail.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id, cancellationToken);
        }

        return null;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await context.ProductDetail
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<DomainProductDetail?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ProductDetail.Include(pd => pd.Product)
            .FirstOrDefaultAsync(pd => pd.Id == id, cancellationToken);
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

    public async Task<DomainProductDetail?> UpdateAsync(DomainProductDetail request,
        CancellationToken cancellationToken = default)
    {
        context.ProductDetail.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
        {
            return await context.ProductDetail.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        }

        return null;
    }
}