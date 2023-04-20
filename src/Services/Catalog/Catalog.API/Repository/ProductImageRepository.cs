using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repository;

public class ProductImageRepository : IProductImageRepository
{
    private readonly CatalogDbContext context;

    public ProductImageRepository(CatalogDbContext context)
    {
        this.context = context;
    }

    public async Task<ProductImage?> Create(ProductImage requestModel)
    {
        context.ProductImage.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.ProductImage.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == requestModel.Id && x.ProductId == requestModel.ProductId);
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
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ProductImage?> Update(Guid parentId, int id, ProductImageBaseRequestModel requestModel)
    {
        var currentProductImageModel = await context.ProductImage.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id && x.ProductId == parentId);
        if (currentProductImageModel is null)
        {
            return null;
        }
        var affected = await context.ProductImage
            .Where(model => model.ProductId == parentId)
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.Id, requestModel.Id)
              .SetProperty(m => m.ProductId, requestModel.ProductId)
              .SetProperty(m => m.ImageUrl, requestModel.ImageUrl)
              .SetProperty(m => m.ImageAlt, requestModel.ImageAlt)
              .SetProperty(m => m.IsMain, requestModel.IsMain)
            );
        await context.Entry(currentProductImageModel).ReloadAsync();
        return affected == 1 ? currentProductImageModel : null;
    }
}
