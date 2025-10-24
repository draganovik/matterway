using Catalog.Api.Domain;
using Catalog.Api.Infrastructure;
using Catalog.Api.Features.ProductImages.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Features.ProductImages.Data;

public class ProductImageRepository(CatalogDbContext context) : IProductImageRepository
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

    public async Task<ProductImage?> Update(Guid parentId, int id, ProductImageBaseRequest request)
    {
        var currentProductImageModel = await context.ProductImage.Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id && x.ProductId == parentId);
        if (currentProductImageModel is null) return null;
        var affected = await context.ProductImage
            .Where(model => model.ProductId == parentId)
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Id, request.Id)
                .SetProperty(m => m.ProductId, request.ProductId)
                .SetProperty(m => m.ImageUrl, request.ImageUrl)
                .SetProperty(m => m.ImageAlt, request.ImageAlt)
                .SetProperty(m => m.IsMain, request.IsMain)
            );
        await context.Entry(currentProductImageModel).ReloadAsync();
        return affected == 1 ? currentProductImageModel : null;
    }
}
