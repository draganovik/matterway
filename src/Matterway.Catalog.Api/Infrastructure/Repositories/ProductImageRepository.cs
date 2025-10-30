using Matterway.Catalog.Api.Application.Repositories;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Repositories;

public class ProductImageRepository(CatalogDb context) : IProductImageRepository
{
    public async Task<ProductImage?> Create(ProductImage requestModel)
    {
        context.ProductImage.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.ProductImage.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id && x.ProductId == requestModel.ProductId);
        }

        return null;
    }

    public async Task<bool> Delete(Guid parentId, int id)
    {
        var affected = await context.ProductImage
            .Where(model => model.ProductId == parentId)
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<ProductImage?> GetById(Guid parentId, int id)
    {
        return await context.ProductImage.FirstOrDefaultAsync(x => x.Id == id && x.ProductId == parentId);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.ProductImage.CountAsync();
    }

    public async Task<ICollection<ProductImage>> Query(int pageIndex, int pageSize)
    {
        return await context.ProductImage.AsNoTracking()
            .Include(x => x.Product)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ProductImage?> UpdateAsync(ProductImage request)
    {
        context.ProductImage.Update(request);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.ProductImage.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ProductId == request.ProductId);
        }

        return null;
    }
}