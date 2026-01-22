using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;

public sealed class EfPgArticleDetailRepository(CatalogDbComposer context) : IArticleDetailRepository
{
    public async Task<ArticleDetail?> Create(ArticleDetail requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ArticleDetail.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ArticleDetail.Include(x => x.Article).Include(x => x.Detail)
                .FirstOrDefaultAsync(
                    x => x.ArticleId == requestModel.ArticleId && x.DetailSlug == requestModel.DetailSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid articleId, string detailSlug, CancellationToken cancellationToken = default)
    {
        var affected = await context.ArticleDetail
            .Where(model => model.ArticleId == articleId && model.DetailSlug == detailSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ArticleDetail?> GetBy(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ArticleDetail
            .Include(pd => pd.Article)
            .Include(pd => pd.Detail)
            .FirstOrDefaultAsync(pd => pd.ArticleId == articleId && pd.DetailSlug == detailSlug, cancellationToken);
    }

    public async Task<ICollection<ArticleDetail>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.ArticleDetail.Include(pd => pd.Article).Include(pd => pd.Detail).AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<ArticleDetail?> Update(Guid articleId, string detailSlug, ArticleDetail request,
        CancellationToken cancellationToken = default)
    {
        context.ArticleDetail.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ArticleDetail
                .Include(x => x.Article)
                .Include(x => x.Detail)
                .FirstOrDefaultAsync(x => x.ArticleId == articleId && x.DetailSlug == detailSlug, cancellationToken);

        return null;
    }
}