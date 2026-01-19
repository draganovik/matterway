using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

public sealed class EfPgArticleRepository(CatalogDbComposer context)
    : IArticleRepository
{
    public async Task<Article?> Create(Article requestModel, CancellationToken cancellationToken = default)
    {
        context.Article.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.Article
                .Include(x => x.Discounts)
                .Include(x => x.ArticleDetails!)
                .ThenInclude(pd => pd!.Detail)
                .Include(x => x.ArticleSpecifications!)
                .ThenInclude(ps => ps!.Specification)
                .Include(x => x.ArticleImages)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id, cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var exists = await context.Article.AnyAsync(p => p.Id == id, cancellationToken);
                if (!exists)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                await context.ArticleImage
                    .Where(model => model.ArticleId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ArticleDetail
                    .Where(model => model.ArticleId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ArticleSpecification
                    .Where(model => model.ArticleId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.Discount
                    .Where(model => model.ArticleId == id)
                    .ExecuteDeleteAsync(cancellationToken);

                var affected = await context.Article
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync(cancellationToken);

                if (affected != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<Article?> GetBy(Guid id, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.Article
            .Include(x => x.Discounts.Where(d => d.ValidFrom <= now && (d.ValidTo == null || d.ValidTo >= now)))
            .Include(x => x.ArticleDetails!)
            .ThenInclude(pd => pd!.Detail)
            .Include(x => x.ArticleSpecifications!)
            .ThenInclude(ps => ps!.Specification)
            .Include(x => x.ArticleImages)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> Count(string? filter,
        CancellationToken cancellationToken = default)
    {
        var articleQuery = context.Article.AsQueryable().ApplyArticleRsql(filter);
        return await articleQuery.CountAsync(cancellationToken);
    }

    public async Task<ICollection<Article>> Query(
        int pageIndex,
        int pageSize,
        string? filter,
        CancellationToken cancellationToken = default)
    {
        var articleQuery = context.Article.AsQueryable().ApplyArticleRsql(filter);
        var now = DateTime.UtcNow;
        return await articleQuery
            .Include(x => x.Discounts.Where(d => d.ValidFrom <= now && (d.ValidTo == null || d.ValidTo >= now)))
            .AsNoTracking()
            .Include(x => x.ArticleImages).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Article?> Update(Article request, CancellationToken cancellationToken = default)
    {
        context.Article.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.Article
                .Include(x => x.Discounts)
                .Include(x => x.ArticleDetails!)
                .ThenInclude(pd => pd.Detail)
                .Include(x => x.ArticleSpecifications)
                .ThenInclude(ps => ps.Specification)
                .Include(x => x.ArticleImages)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return null;
    }
}