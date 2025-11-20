using Microsoft.EntityFrameworkCore;
using DomainProductDetailType = Matterway.Catalog.Api.Domain.Entities.ProductDetailType;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;

public sealed class ProductDetailTypeRepository(CatalogDb context) : IProductDetailTypeRepository
{
    private const int MaxLimit = 50;
    private const int DefaultLimit = 10;

    public async Task<ICollection<DomainProductDetailType>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default)
    {
        var normalizedLimit = Math.Clamp(limit <= 0 ? DefaultLimit : limit, 1, MaxLimit);
        var query = context.ProductDetailType.AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleLike))
        {
            var normalized = titleLike.Trim().ToLower();
            query = query.Where(pdt => pdt.Title.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(pdt => pdt.Title)
            .Take(normalizedLimit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainProductDetailType?> GetBySlugAsync(string slug,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductDetailType.AsNoTracking()
            .FirstOrDefaultAsync(pdt => pdt.Slug == slug, cancellationToken);
    }
}