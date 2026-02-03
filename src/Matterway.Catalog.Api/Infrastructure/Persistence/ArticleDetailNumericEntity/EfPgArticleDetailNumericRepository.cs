using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;

public sealed class EfPgArticleDetailNumericRepository(CatalogDbComposer context) : IArticleDetailNumericRepository
{
    public async Task<ArticleDetailNumeric?> Create(ArticleDetailNumeric requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ArticleDetailNumeric.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.ArticleDetailNumeric.Include(x => x.Article).Include(x => x.Detail)
                .FirstOrDefaultAsync(
                    x => x.ArticleId == requestModel.ArticleId && x.DetailSlug == requestModel.DetailSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid articleId, string detailSlug, CancellationToken cancellationToken = default)
    {
        var affected = await context.ArticleDetailNumeric
            .Where(model => model.ArticleId == articleId && model.DetailSlug == detailSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ArticleDetailNumeric?> GetBy(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ArticleDetailNumeric
            .Include(pd => pd.Detail)
            .FirstOrDefaultAsync(pd => pd.ArticleId == articleId && pd.DetailSlug == detailSlug, cancellationToken);
    }

    public async Task<ArticleDetailNumeric?> Update(Guid articleId, string detailSlug, ArticleDetailNumeric request,
        CancellationToken cancellationToken = default)
    {
        context.ArticleDetailNumeric.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.ArticleDetailNumeric
                .Include(x => x.Detail)
                .FirstOrDefaultAsync(x => x.ArticleId == articleId && x.DetailSlug == detailSlug, cancellationToken);

        return null;
    }
}