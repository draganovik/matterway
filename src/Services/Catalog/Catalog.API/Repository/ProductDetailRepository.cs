using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repository;

public class ProductDetailRepository : IProductDetailRepository
{
    private readonly CatalogDbContext context;

    public ProductDetailRepository(CatalogDbContext context)
    {
        this.context = context;
    }

    public async Task<ProductDetail?> Create(ProductDetail requestModel)
    {
        context.ProductDetail.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.ProductDetail.FindAsync(requestModel.Id);
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
        return await context.ProductDetail.FindAsync(id);
    }

    public async Task<ICollection<ProductDetail>> Query()
    {
        return await context.ProductDetail.AsNoTracking()
        .ToListAsync();
    }

    public async Task<ProductDetail?> Update(Guid id, ProductDetailBaseRequestModel requestModel)
    {
        var currentProductDetailModel = await context.ProductDetail.FindAsync(id);
        if (currentProductDetailModel is null)
        {
            return null;
        }
        var affected = await context.ProductDetail
        .Where(model => model.Id == id)
        .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.Type, requestModel.Type)
              .SetProperty(m => m.ProductId, requestModel.ProductId)
              .SetProperty(m => m.Title, requestModel.Title)
              .SetProperty(m => m.Value, requestModel.Value)
              .SetProperty(m => m.Unit, requestModel.Unit)
            );
        await context.Entry(currentProductDetailModel).ReloadAsync();
        return affected == 1 ? currentProductDetailModel : null;
    }
}
