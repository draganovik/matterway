using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;

public sealed class EfPgArticleSpecificationRepository(CatalogDbComposer context) : IArticleSpecificationRepository
{
    public async Task<ArticleSpecification?> Create(ArticleSpecification requestModel,
        CancellationToken cancellationToken = default)
    {
        context.ArticleSpecification.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ArticleSpecification
                .Include(x => x.Article)
                .Include(x => x.Specification)
                .FirstOrDefaultAsync(x => x.ArticleId == requestModel.ArticleId &&
                                          x.SpecificationSlug == requestModel.SpecificationSlug,
                    cancellationToken);

        return null;
    }

    public async Task<bool> Delete(Guid articleId, string specificationSlug,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.ArticleSpecification
            .Where(model => model.ArticleId == articleId && model.SpecificationSlug == specificationSlug)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<ArticleSpecification?> GetBy(Guid articleId, string specificationSlug,
        CancellationToken cancellationToken = default)
    {
        return await context.ArticleSpecification
            .Include(ps => ps.Article)
            .Include(ps => ps.Specification)
            .FirstOrDefaultAsync(ps => ps.ArticleId == articleId && ps.SpecificationSlug == specificationSlug,
                cancellationToken);
    }

    public async Task<ArticleSpecification?> Update(Guid articleId, string specificationSlug,
        ArticleSpecification request, CancellationToken cancellationToken = default)
    {
        context.ArticleSpecification.Update(request);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1)
            return await context.ArticleSpecification
                .Include(x => x.Article)
                .Include(x => x.Specification)
                .FirstOrDefaultAsync(
                    x => x.ArticleId == articleId && x.SpecificationSlug == specificationSlug,
                    cancellationToken);

        return null;
    }
}