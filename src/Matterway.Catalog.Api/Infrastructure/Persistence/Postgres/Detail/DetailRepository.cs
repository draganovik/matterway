using Microsoft.EntityFrameworkCore;
using DomainDetail = Matterway.Catalog.Api.Domain.Entities.Detail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Detail;

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

    public async Task<DomainDetail?> UpsertAsync(DomainDetail requestModel,
        CancellationToken cancellationToken = default)
    {
        var existing = await context.Detail.FirstOrDefaultAsync(d => d.Slug == requestModel.Slug, cancellationToken);
        if (existing is null)
        {
            context.Detail.Add(requestModel);
        }
        else
        {
            existing.Title = requestModel.Title;
            context.Detail.Update(existing);
        }

        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected <= 0) return null;

        return await context.Detail.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Slug == requestModel.Slug, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string slug, CancellationToken cancellationToken = default)
    {
        var inUse = await context.ProductDetail.AnyAsync(pd => pd.DetailSlug == slug, cancellationToken);
        if (inUse) return false;

        var entity = await context.Detail.FirstOrDefaultAsync(d => d.Slug == slug, cancellationToken);
        if (entity is null) return false;

        context.Detail.Remove(entity);
        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0;
    }
}