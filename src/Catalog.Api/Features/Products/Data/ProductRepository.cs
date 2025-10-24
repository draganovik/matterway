using Catalog.Api.Domain;
using Catalog.Api.Infrastructure;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Shared;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Features.Products.Data;

public class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public async Task<Product?> Create(Product requestModel)
    {
        context.Product.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id);
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
        return await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> GetTotalEntities(ProductFilter productFilter)
    {
        var productQuery = productFilter.GenerateQuery(context.Product.AsQueryable());
        return await productQuery.CountAsync();
    }

    public async Task<ICollection<Product>> Query(int pageIndex, int pageSize, ProductFilter productFilter)
    {
        var productQuery = productFilter.GenerateQuery(context.Product.AsQueryable());
        return await productQuery.Include(x => x.ProductDetails).Include(x => x.ProductImages).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> Update(Guid id, ProductBaseRequest request)
    {
        var currentProductModel = await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (currentProductModel is null) return null;
        var affected = await context.Product
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ProductCode, request.ProductCode)
                .SetProperty(m => m.Title, request.Title)
                .SetProperty(m => m.Price, request.Price)
                .SetProperty(m => m.Description, request.Description)
                .SetProperty(m => m.IsAvailable, request.IsAvailable)
            );
        await context.Entry(currentProductModel).ReloadAsync();
        return affected == 1 ? currentProductModel : null;
    }
}
