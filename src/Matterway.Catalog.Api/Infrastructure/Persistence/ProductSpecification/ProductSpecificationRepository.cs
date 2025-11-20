using Microsoft.EntityFrameworkCore;
using DomainProductSpecification = Matterway.Catalog.Api.Domain.Entities.ProductSpecification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductSpecification;

public sealed class ProductSpecificationRepository(CatalogDb context) : IProductSpecificationRepository
{
    public async Task<DomainProductSpecification?> Create(DomainProductSpecification requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ProductSpecification.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ProductSpecification
                .Include(x => x.Product)
                .Include(x => x.Specification)
                .FirstOrDefaultAsync(x => x.ProductId == requestModel.ProductId &&
                                          x.SpecificationSlug == requestModel.SpecificationSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid productId, string specificationSlug,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.ProductSpecification
            .Where(model => model.ProductId == productId && model.SpecificationSlug == specificationSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<DomainProductSpecification?> GetByKey(Guid productId, string specificationSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductSpecification
            .Include(ps => ps.Product)
            .Include(ps => ps.Specification)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.SpecificationSlug == specificationSlug,
                cancellationToken);
    }

    public async Task<int> GetTotalEntities(CancellationToken cancellationToken = default)
    {
        return await context.ProductSpecification.CountAsync(cancellationToken);
    }

    public async Task<ICollection<DomainProductSpecification>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductSpecification.Include(ps => ps.Product).Include(ps => ps.Specification)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainProductSpecification?> UpdateAsync(Guid productId, string specificationSlug,
        DomainProductSpecification request, CancellationToken cancellationToken = default)
    {
        context.ProductSpecification.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ProductSpecification
                .Include(x => x.Product)
                .Include(x => x.Specification)
                .FirstOrDefaultAsync(
                    x => x.ProductId == productId && x.SpecificationSlug == specificationSlug,
                    cancellationToken);

        return null;
    }
}