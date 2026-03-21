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
        return affected > 0 ? requestModel : null;
    }

    public async Task<bool> Delete(ArticleCode code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Value;
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var exists = await context.Article.AnyAsync(p => p.ArticleCode == normalizedCode, cancellationToken);
                if (!exists)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                await context.ArticleImage
                    .Where(model => model.ArticleCode == normalizedCode)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ArticleDetailText
                    .Where(model => model.ArticleCode == normalizedCode)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.ArticleDetailNumeric
                    .Where(model => model.ArticleCode == normalizedCode)
                    .ExecuteDeleteAsync(cancellationToken);

                await context.Discount
                    .Where(model => model.ArticleCode == normalizedCode)
                    .ExecuteDeleteAsync(cancellationToken);

                var affected = await context.Article
                    .Where(model => model.ArticleCode == normalizedCode)
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

    public async Task<Article?> GetBy(ArticleCode code, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.Article
            .Include(x => x.Discounts.Where(d => d.ValidFrom <= now && (d.ValidTo == null || d.ValidTo >= now)))
            .Include(x => x.ArticleDetailTexts!)
            .ThenInclude(pd => pd!.Detail)
            .Include(x => x.ArticleDetailNumerics!)
            .ThenInclude(pd => pd!.Detail)
            .Include(x => x.ArticleImages)
            .FirstOrDefaultAsync(x => x.ArticleCode == code.Value, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Article>> GetByCodes(IEnumerable<ArticleCode> codes,
        CancellationToken cancellationToken = default)
    {
        var normalizedCodes = codes
            .Select(code => code.Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (normalizedCodes.Length == 0) return [];

        return await context.Article
            .Where(article => normalizedCodes.Contains(article.ArticleCode))
            .ToListAsync(cancellationToken);
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
            .AsNoTracking()
            .Include(x => x.Discounts.Where(d => d.ValidFrom <= now && (d.ValidTo == null || d.ValidTo >= now)))
            .Include(x => x.ArticleImages)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.ArticleCode)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Article?> Update(Article request, ArticleCode originalCode,
        CancellationToken cancellationToken = default)
    {
        var originalCodeValue = originalCode.Value;
        var nextCodeValue = request.ArticleCode;

        if (string.Equals(originalCodeValue, nextCodeValue, StringComparison.Ordinal))
        {
            context.Article.Update(request);
            var affected = await context.SaveChangesAsync(cancellationToken);
            if (affected > 0)
                return await context.Article
                    .Include(x => x.Discounts)
                    .Include(x => x.ArticleDetailTexts!)
                    .ThenInclude(pd => pd.Detail)
                    .Include(x => x.ArticleDetailNumerics)
                    .ThenInclude(pd => pd.Detail)
                    .Include(x => x.ArticleImages)
                    .FirstOrDefaultAsync(x => x.ArticleCode == request.ArticleCode, cancellationToken);

            return null;
        }

        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var replacement = new Article
                {
                    ArticleCode = nextCodeValue,
                    Title = request.Title,
                    Description = request.Description,
                    BasePrice = request.BasePrice,
                    IsAvailable = request.IsAvailable,
                    CreatedAt = request.CreatedAt,
                    UpdatedAt = request.UpdatedAt
                };

                context.Article.Add(replacement);
                await context.SaveChangesAsync(cancellationToken);

                await context.ArticleImage
                    .Where(model => model.ArticleCode == originalCodeValue)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(model => model.ArticleCode, nextCodeValue), cancellationToken);

                await context.ArticleDetailText
                    .Where(model => model.ArticleCode == originalCodeValue)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(model => model.ArticleCode, nextCodeValue), cancellationToken);

                await context.ArticleDetailNumeric
                    .Where(model => model.ArticleCode == originalCodeValue)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(model => model.ArticleCode, nextCodeValue), cancellationToken);

                await context.Discount
                    .Where(model => model.ArticleCode == originalCodeValue)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(model => model.ArticleCode, nextCodeValue), cancellationToken);

                await context.Article
                    .Where(model => model.ArticleCode == originalCodeValue)
                    .ExecuteDeleteAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return await context.Article
                    .Include(x => x.Discounts)
                    .Include(x => x.ArticleDetailTexts!)
                    .ThenInclude(pd => pd.Detail)
                    .Include(x => x.ArticleDetailNumerics)
                    .ThenInclude(pd => pd.Detail)
                    .Include(x => x.ArticleImages)
                    .FirstOrDefaultAsync(x => x.ArticleCode == nextCodeValue, cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}