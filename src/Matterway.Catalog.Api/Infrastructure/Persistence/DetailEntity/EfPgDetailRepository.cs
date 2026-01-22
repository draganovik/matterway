using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

public sealed class EfPgDetailRepository(CatalogDbComposer context) : IDetailRepository
{
    private const int MaxLimit = 50;
    private const int DefaultLimit = 10;

    public async Task<ICollection<Detail>> Query(string? titleLike, int limit,
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

    public async Task<Detail?> GetBy(string slug, CancellationToken cancellationToken = default)
    {
        return await context.Detail.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Slug == slug, cancellationToken);
    }

    public async Task<Detail?> Upsert(Detail requestModel,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlug = requestModel.Slug.Trim().ToLower();
        requestModel = new Detail
        {
            Slug = normalizedSlug,
            Title = requestModel.Title.Trim()
        };

        var inUseBySpecification =
            await context.Specification.AnyAsync(s => s.Slug == normalizedSlug, cancellationToken);
        if (inUseBySpecification)
            return null;

        var existing = await context.Detail.FirstOrDefaultAsync(d => d.Slug == normalizedSlug, cancellationToken);
        if (existing is null)
        {
            if (!await context.AttributeSlug.AnyAsync(a => a.Slug == normalizedSlug, cancellationToken))
                context.AttributeSlug.Add(new AttributeSlug { Slug = normalizedSlug });

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

    public async Task<bool> Delete(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.Trim().ToLower();
        var inUse = await context.ArticleDetail.AnyAsync(pd => pd.DetailSlug == normalizedSlug, cancellationToken);
        if (inUse) return false;

        var entity = await context.Detail.FirstOrDefaultAsync(d => d.Slug == normalizedSlug, cancellationToken);
        if (entity is null) return false;

        context.Detail.Remove(entity);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected <= 0) return false;

        var stillUsedBySpecifications =
            await context.Specification.AnyAsync(s => s.Slug == normalizedSlug, cancellationToken);
        if (!stillUsedBySpecifications)
        {
            var attr = await context.AttributeSlug.FirstOrDefaultAsync(a => a.Slug == normalizedSlug,
                cancellationToken);
            if (attr is not null)
            {
                context.AttributeSlug.Remove(attr);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        return true;
    }
}