using Microsoft.EntityFrameworkCore;
using DomainSpecification = Matterway.Catalog.Api.Domain.Entities.Specification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Specification;

public sealed class SpecificationRepository(CatalogDb context) : ISpecificationRepository
{
    private const int MaxLimit = 50;
    private const int DefaultLimit = 10;

    public async Task<ICollection<DomainSpecification>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default)
    {
        var normalizedLimit = Math.Clamp(limit <= 0 ? DefaultLimit : limit, 1, MaxLimit);
        var query = context.Specification.AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleLike))
        {
            var normalized = titleLike.Trim().ToLower();
            query = query.Where(s => s.Title.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(s => s.Title)
            .Take(normalizedLimit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainSpecification?> GetBySlugAsync(string slug,
        CancellationToken cancellationToken = default)
    {
        return await context.Specification.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken);
    }
}