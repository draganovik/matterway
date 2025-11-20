using Microsoft.EntityFrameworkCore;
using DomainDetail = Matterway.Catalog.Api.Domain.Entities.Detail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Detail;

public sealed class DetailRepository(CatalogDb context) : IDetailRepository
{
    private const int MaxLimit = 50;
    private const int DefaultLimit = 10;

    public async Task<ICollection<DomainDetail>> QueryAsync(string? titleLike, int limit,
        CancellationToken cancellationToken = default)
    {
        var normalizedLimit = Math.Clamp(limit <= 0 ? DefaultLimit : limit, 1, MaxLimit);
        var query = context.Detail.AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleLike))
        {
            var normalized = titleLike.Trim().ToLower();
            query = query.Where(d => d.Title.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(d => d.Title)
            .Take(normalizedLimit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainDetail?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await context.Detail.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Slug == slug, cancellationToken);
    }
}