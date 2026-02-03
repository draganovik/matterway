using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;

public sealed class EfPgArticleDetailTextRepository(CatalogDbComposer context) : IArticleDetailTextRepository
{
    public async Task<ArticleDetailText?> Create(ArticleDetailText requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ArticleDetailText.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.ArticleDetailText.Include(x => x.Article).Include(x => x.Detail)
                .FirstOrDefaultAsync(
                    x => x.ArticleId == requestModel.ArticleId && x.DetailSlug == requestModel.DetailSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid articleId, string detailSlug, CancellationToken cancellationToken = default)
    {
        var affected = await context.ArticleDetailText
            .Where(model => model.ArticleId == articleId && model.DetailSlug == detailSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ArticleDetailText?> GetBy(Guid articleId, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ArticleDetailText
            .Include(pd => pd.Detail)
            .FirstOrDefaultAsync(pd => pd.ArticleId == articleId && pd.DetailSlug == detailSlug, cancellationToken);
    }

    public async Task<ArticleDetailText?> Update(Guid articleId, string detailSlug, ArticleDetailText request,
        CancellationToken cancellationToken = default)
    {
        context.ArticleDetailText.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.ArticleDetailText
                .Include(x => x.Detail)
                .FirstOrDefaultAsync(x => x.ArticleId == articleId && x.DetailSlug == detailSlug, cancellationToken);

        return null;
    }
}