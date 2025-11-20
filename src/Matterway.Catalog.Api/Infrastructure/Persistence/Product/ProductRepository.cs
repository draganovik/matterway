using Matterway.Catalog.Api.Domain;
using Microsoft.EntityFrameworkCore;
using DomainProduct = Matterway.Catalog.Api.Domain.Entities.Product;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Product;

public sealed class ProductRepository(CatalogDb context)
    : IProductRepository
{
    public async Task<DomainProduct?> Create(DomainProduct requestModel, CancellationToken cancellationToken = default)
    {
        context.Product.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.Product
                .Include(x => x.ProductDetails!)
                .ThenInclude(pd => pd!.Detail)
                .Include(x => x.ProductSpecifications!)
                .ThenInclude(ps => ps!.Specification)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id, cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await context.Product
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<DomainProduct?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Product
            .Include(x => x.ProductDetails!)
            .ThenInclude(pd => pd!.Detail)
            .Include(x => x.ProductSpecifications!)
            .ThenInclude(ps => ps!.Specification)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalEntities(string? filter,
        CancellationToken cancellationToken = default)
    {
        var productQuery = context.Product.AsQueryable().ApplyProductRsql(filter);
        return await productQuery.CountAsync(cancellationToken);
    }

    public async Task<ICollection<DomainProduct>> Query(
        int pageIndex,
        int pageSize,
        string? filter,
        CancellationToken cancellationToken = default)
    {
        var productQuery = context.Product.AsQueryable().ApplyProductRsql(filter);
        return await productQuery.Include(x => x.ProductImages).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainProduct?> UpdateAsync(DomainProduct request, CancellationToken cancellationToken = default)
    {
        context.Product.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.Product
                .Include(x => x.ProductDetails!)
                .ThenInclude(pd => pd!.Detail)
                .Include(x => x.ProductSpecifications!)
                .ThenInclude(ps => ps!.Specification)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return null;
    }
}