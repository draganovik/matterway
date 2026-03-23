using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

public sealed class EfPgDetailRepository(CatalogDbComposer context) : IDetailRepository
{
    public async Task<int> Count(string? titleLike, CancellationToken cancellationToken = default)
    {
        return await BuildQuery(titleLike)
            .CountAsync(cancellationToken);
    }

    public async Task<ICollection<Detail>> Query(int page, int pageSize, string? titleLike,
        CancellationToken cancellationToken = default)
    {
        var offset = (page - 1) * pageSize;

        return await BuildQuery(titleLike)
            .AsNoTracking()
            .OrderBy(d => d.Title)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Detail> BuildQuery(string? titleLike)
    {
        var query = context.Detail.AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleLike))
        {
            var normalized = titleLike.Trim().ToLower();
            query = query.Where(d => d.Title.ToLower().Contains(normalized));
        }

        return query;
    }

    public async Task<Detail?> GetBy(string slug, CancellationToken cancellationToken = default)
    {
        return await context.Detail.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Slug == slug, cancellationToken);
    }

    public async Task<Detail?> Upsert(Detail requestModel,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlug = requestModel.Slug.Trim().ToLower();
        var normalizedUnit = string.IsNullOrWhiteSpace(requestModel.Unit)
            ? null
            : requestModel.Unit.Trim();
        requestModel = new Detail
        {
            Slug = normalizedSlug,
            Title = requestModel.Title.Trim(),
            Unit = normalizedUnit
        };

        var existing = await context.Detail.FirstOrDefaultAsync(d => d.Slug == normalizedSlug, cancellationToken);
        if (existing is null)
        {
            context.Detail.Add(requestModel);
        }
        else
        {
            var unitChanged = !string.Equals(existing.Unit, requestModel.Unit, StringComparison.Ordinal);
            if (unitChanged)
            {
                if (requestModel.Unit is not null)
                {
                    var hasTextDetails = await context.ArticleDetailText
                        .AnyAsync(d => d.DetailSlug == normalizedSlug, cancellationToken);
                    if (hasTextDetails) return null;
                }
                else
                {
                    var hasNumericDetails = await context.ArticleDetailNumeric
                        .AnyAsync(d => d.DetailSlug == normalizedSlug, cancellationToken);
                    if (hasNumericDetails) return null;
                }
            }

            existing.Title = requestModel.Title;
            existing.Unit = requestModel.Unit;
        }

        await context.SaveChangesAsync(cancellationToken);
        return existing ?? requestModel;
    }

    public async Task<bool> Delete(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.Trim().ToLower();
        var inUse = await context.ArticleDetailText.AnyAsync(pd => pd.DetailSlug == normalizedSlug, cancellationToken)
                    || await context.ArticleDetailNumeric.AnyAsync(pd => pd.DetailSlug == normalizedSlug,
                        cancellationToken);
        if (inUse) return false;

        var entity = await context.Detail.FirstOrDefaultAsync(d => d.Slug == normalizedSlug, cancellationToken);
        if (entity is null) return false;

        context.Detail.Remove(entity);
        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0;
    }
}