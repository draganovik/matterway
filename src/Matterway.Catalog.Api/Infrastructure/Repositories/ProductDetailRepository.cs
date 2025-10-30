using Matterway.Catalog.Api.Application.Repositories;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Repositories;

public class ProductDetailRepository(CatalogDb context) : IProductDetailRepository
{
    public async Task<ProductDetail?> Create(ProductDetail requestModel)
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

    public async Task<ProductDetail?> GetById(Guid id)
    {
        return await context.ProductDetail.Include(pd => pd.Product).FirstOrDefaultAsync(pd => pd.Id == id);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.ProductDetail.CountAsync();
    }

    public async Task<ICollection<ProductDetail>> Query(int pageIndex, int pageSize)
    {
        return await context.ProductDetail.Include(pd => pd.Product).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ProductDetail?> UpdateAsync(ProductDetail request)
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