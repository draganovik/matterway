using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Providers.Persistence.SpecificationEntity;

public sealed class EfPgSpecificationRepository(CatalogDb context) : ISpecificationRepository
{
    private const int MaxLimit = 50;
    private const int DefaultLimit = 10;

    public async Task<ICollection<Specification>> Query(string? titleLike, int limit,
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

    public async Task<Specification?> GetBy(string slug,
        CancellationToken cancellationToken = default)
    {
        return await context.Specification.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken);
    }

    public async Task<Specification?> Upsert(Specification requestModel,
        CancellationToken cancellationToken = default)
    {
        var normalizedSlug = requestModel.Slug.Trim().ToLower();
        requestModel = new Specification
        {
            Slug = normalizedSlug,
            Title = requestModel.Title.Trim(),
            Unit = requestModel.Unit?.Trim()
        };

        var inUseByDetail = await context.Detail.AnyAsync(d => d.Slug == normalizedSlug, cancellationToken);
        if (inUseByDetail)
            return null;

        var existing = await context.Specification.FirstOrDefaultAsync(s => s.Slug == normalizedSlug,
            cancellationToken);
        if (existing is null)
        {
            if (!await context.AttributeSlug.AnyAsync(a => a.Slug == normalizedSlug, cancellationToken))
                context.AttributeSlug.Add(new AttributeSlug { Slug = normalizedSlug });

            context.Specification.Add(requestModel);
        }
        else
        {
            existing.Title = requestModel.Title;
            existing.Unit = requestModel.Unit;
            context.Specification.Update(existing);
        }

        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected <= 0) return null;

        return await context.Specification.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == requestModel.Slug, cancellationToken);
    }

    public async Task<bool> Delete(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.Trim().ToLower();
        var inUse = await context.ProductSpecification.AnyAsync(ps => ps.SpecificationSlug == normalizedSlug,
            cancellationToken);
        if (inUse) return false;

        var entity = await context.Specification.FirstOrDefaultAsync(s => s.Slug == normalizedSlug, cancellationToken);
        if (entity is null) return false;

        context.Specification.Remove(entity);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected <= 0) return false;

        var stillUsedByDetails = await context.Detail.AnyAsync(d => d.Slug == normalizedSlug, cancellationToken);
        if (!stillUsedByDetails)
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