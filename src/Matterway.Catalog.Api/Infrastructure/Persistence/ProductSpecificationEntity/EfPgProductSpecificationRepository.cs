using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductSpecificationEntity;

public sealed class EfPgProductSpecificationRepository(CatalogDb context) : IProductSpecificationRepository
{
    public async Task<ProductSpecification?> Create(ProductSpecification requestModel,
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

    public async Task<ProductSpecification?> GetBy(Guid productId, string specificationSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductSpecification
            .Include(ps => ps.Product)
            .Include(ps => ps.Specification)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.SpecificationSlug == specificationSlug,
                cancellationToken);
    }

    public async Task<ProductSpecification?> Update(Guid productId, string specificationSlug,
        ProductSpecification request, CancellationToken cancellationToken = default)
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