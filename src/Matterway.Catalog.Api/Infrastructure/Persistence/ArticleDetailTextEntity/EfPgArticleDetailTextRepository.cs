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
                    x => x.ArticleCode == requestModel.ArticleCode && x.DetailSlug == requestModel.DetailSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        var affected = await context.ArticleDetailText
            .Where(model => model.ArticleCode == normalizedCode && model.DetailSlug == detailSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ArticleDetailText?> GetBy(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        return await context.ArticleDetailText
            .Include(pd => pd.Detail)
            .FirstOrDefaultAsync(pd => pd.ArticleCode == normalizedCode && pd.DetailSlug == detailSlug,
                cancellationToken);
    }

    public async Task<ArticleDetailText?> Update(ArticleCode articleCode, string detailSlug, ArticleDetailText request,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        context.ArticleDetailText.Update(request);
        await context.SaveChangesAsync(cancellationToken);
        return await context.ArticleDetailText
            .Include(x => x.Detail)
            .FirstOrDefaultAsync(x => x.ArticleCode == normalizedCode && x.DetailSlug == detailSlug,
                cancellationToken);
    }
}
