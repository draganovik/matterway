using Microsoft.EntityFrameworkCore;
using DomainProductDetail = Matterway.Catalog.Api.Domain.ProductDetail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;

public sealed class ProductDetailRepository(CatalogDb context) : IProductDetailRepository
{
    public async Task<DomainProductDetail?> Create(DomainProductDetail requestModel)
    {
        context.ProductDetail.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.ProductDetail.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id);
        }

        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.ProductDetail
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<DomainProductDetail?> GetById(Guid id)
    {
        return await context.ProductDetail.Include(pd => pd.Product).FirstOrDefaultAsync(pd => pd.Id == id);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.ProductDetail.CountAsync();
    }

    public async Task<ICollection<DomainProductDetail>> Query(int pageIndex, int pageSize)
    {
        return await context.ProductDetail.Include(pd => pd.Product).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<DomainProductDetail?> UpdateAsync(DomainProductDetail request)
    {
        context.ProductDetail.Update(request);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.ProductDetail.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
        }

        return null;
    }
}