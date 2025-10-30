using Matterway.Catalog.Api.Application.Repositories;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.Products.Query;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Repositories;

public class ProductRepository(CatalogDb context) : IProductRepository
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

    public async Task<int> GetTotalEntities(QueryProductFilter queryProductFilter)
    {
        var productQuery = queryProductFilter.GenerateQuery(context.Product.AsQueryable());
        return await productQuery.CountAsync();
    }

    public async Task<ICollection<Product>> Query(int pageIndex, int pageSize, QueryProductFilter queryProductFilter)
    {
        var productQuery = queryProductFilter.GenerateQuery(context.Product.AsQueryable());
        return await productQuery.Include(x => x.ProductImages).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> UpdateAsync(Product request)
    {
        context.Product.Update(request);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.Product.Include(x => x.ProductDetails).Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
        }

        return null;
    }
}