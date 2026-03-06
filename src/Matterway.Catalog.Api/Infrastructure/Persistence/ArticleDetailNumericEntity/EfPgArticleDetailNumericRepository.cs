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
                    x => x.ArticleCode == requestModel.ArticleCode && x.DetailSlug == requestModel.DetailSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        var affected = await context.ArticleDetailNumeric
            .Where(model => model.ArticleCode == normalizedCode && model.DetailSlug == detailSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ArticleDetailNumeric?> GetBy(ArticleCode articleCode, string detailSlug,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        return await context.ArticleDetailNumeric
            .Include(pd => pd.Detail)
            .FirstOrDefaultAsync(pd => pd.ArticleCode == normalizedCode && pd.DetailSlug == detailSlug,
                cancellationToken);
    }

    public async Task<ArticleDetailNumeric?> Update(ArticleCode articleCode, string detailSlug,
        ArticleDetailNumeric request,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        context.ArticleDetailNumeric.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.ArticleDetailNumeric
                .Include(x => x.Detail)
                .FirstOrDefaultAsync(x => x.ArticleCode == normalizedCode && x.DetailSlug == detailSlug,
                    cancellationToken);

        return null;
    }
}