using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.API.Filters;
using Catalog.API.Models.ProductModels;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repository;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext context;

    public ProductRepository(CatalogDbContext context)
    {
        this.context = context;
    }

    public async Task<Product?> Create(Product requestModel)
    {
        context.Product.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == requestModel.Id);
        }
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.Product
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<Product?> GetById(Guid id)
    {
        return await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.Product.CountAsync();
    }

    public async Task<ICollection<Product>> Query(int pageIndex, int pageSize, ProductFilter productFilter)
    {
        var productQuery = productFilter.GenerateQuery(context.Product.AsQueryable());
        return await productQuery.Include(x => x.ProductDetails).Include(x => x.ProductImages).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> Update(Guid id, ProductBaseRequestModel requestModel)
    {
        var currentProductModel = await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (currentProductModel is null)
        {
            return null;
        }
        var affected = await context.Product
        .Where(model => model.Id == id)
        .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.ProductCode, requestModel.ProductCode)
              .SetProperty(m => m.Title, requestModel.Title)
              .SetProperty(m => m.Price, requestModel.Price)
              .SetProperty(m => m.Description, requestModel.Description)
              .SetProperty(m => m.IsAvailable, requestModel.IsAvailable)
            );
        await context.Entry(currentProductModel).ReloadAsync();
        return affected == 1 ? currentProductModel : null;
    }
}
