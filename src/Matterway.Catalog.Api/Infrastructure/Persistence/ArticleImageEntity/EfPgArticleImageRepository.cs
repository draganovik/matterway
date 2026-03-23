using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

public sealed class EfPgArticleImageRepository(CatalogDbComposer context) : IArticleImageRepository
{
    public async Task<ArticleImage?> Create(ArticleImage requestModel, CancellationToken cancellationToken)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existingCount = await context.ArticleImage
                    .Where(pi => pi.ArticleCode == requestModel.ArticleCode)
                    .CountAsync(cancellationToken);
                var maxOrderIndex = await GetMaxOrderIndex(requestModel.ArticleCode, cancellationToken);

                requestModel.OrderIndex = Math.Clamp(requestModel.OrderIndex, 0, existingCount);

                if (maxOrderIndex is not null)
                    await ShiftOrderRange(
                        requestModel.ArticleCode,
                        requestModel.OrderIndex,
                        maxOrderIndex.Value,
                        1,
                        cancellationToken);

                context.ArticleImage.Add(requestModel);
                var affected = await context.SaveChangesAsync(cancellationToken);
                if (affected != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                await transaction.CommitAsync(cancellationToken);

                return await context.ArticleImage.Include(x => x.Article)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == requestModel.Id && x.ArticleCode == requestModel.ArticleCode,
                        cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<bool> Delete(ArticleCode articleCode, int orderIndex,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var affected = await context.ArticleImage
                    .Where(model => model.ArticleCode == normalizedCode)
                    .Where(model => model.OrderIndex == orderIndex)
                    .ExecuteDeleteAsync(cancellationToken);
                if (affected != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                var maxOrderIndex = await GetMaxOrderIndex(normalizedCode, cancellationToken);
                if (maxOrderIndex is not null)
                    await ShiftOrderRange(
                        normalizedCode,
                        orderIndex + 1,
                        maxOrderIndex.Value,
                        -1,
                        cancellationToken);

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

    public async Task<ArticleImage?> GetBy(ArticleCode articleCode, int orderIndex,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        return await
            context.ArticleImage.FirstOrDefaultAsync(x => x.OrderIndex == orderIndex && x.ArticleCode == normalizedCode,
                cancellationToken);
    }

    public async Task<ICollection<ArticleImage>> GetByArticle(ArticleCode articleCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = articleCode.Value;
        return await context.ArticleImage
            .AsNoTracking()
            .Where(x => x.ArticleCode == normalizedCode)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<ArticleImage?> Update(ArticleImage request, int targetOrderIndex,
        CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existing = await context.ArticleImage
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ArticleCode == request.ArticleCode && x.Id == request.Id,
                        cancellationToken);

                if (existing is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                var total = await context.ArticleImage
                    .Where(x => x.ArticleCode == request.ArticleCode)
                    .CountAsync(cancellationToken);

                if (total == 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                var clampedTarget = Math.Clamp(targetOrderIndex, 0, total - 1);
                var newAlt = request.ImageAlt ?? existing.ImageAlt;

                if (clampedTarget != existing.OrderIndex)
                {
                    // Move the current row out of the indexed range before reordering neighbors.
                    var temporaryOrderIndex =
                        await GetTemporaryCurrentOrderIndex(request.ArticleCode, cancellationToken);
                    await context.ArticleImage
                        .Where(x => x.ArticleCode == request.ArticleCode && x.Id == request.Id)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(x => x.OrderIndex, _ => temporaryOrderIndex), cancellationToken);

                    if (clampedTarget < existing.OrderIndex)
                        await ShiftOrderRange(
                            request.ArticleCode,
                            clampedTarget,
                            existing.OrderIndex - 1,
                            1,
                            cancellationToken);
                    else
                        await ShiftOrderRange(
                            request.ArticleCode,
                            existing.OrderIndex + 1,
                            clampedTarget,
                            -1,
                            cancellationToken);
                }

                var updatedRows = await context.ArticleImage
                    .Where(x => x.ArticleCode == request.ArticleCode && x.Id == request.Id)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(x => x.OrderIndex, x => clampedTarget)
                            .SetProperty(x => x.ImageAlt, x => newAlt), cancellationToken);

                if (updatedRows != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                await transaction.CommitAsync(cancellationToken);

                return await context.ArticleImage.Include(x => x.Article)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ArticleCode == request.ArticleCode && x.Id == request.Id,
                        cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    private async Task ShiftOrderRange(
        string articleCode,
        int startOrderIndex,
        int endOrderIndex,
        int delta,
        CancellationToken cancellationToken)
    {
        if (startOrderIndex > endOrderIndex) return;

        var temporaryOffset = await GetTemporaryOffset(articleCode, cancellationToken);

        await context.ArticleImage
            .Where(x => x.ArticleCode == articleCode)
            .Where(x => x.OrderIndex >= startOrderIndex && x.OrderIndex <= endOrderIndex)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.OrderIndex, x => x.OrderIndex + temporaryOffset), cancellationToken);

        await context.ArticleImage
            .Where(x => x.ArticleCode == articleCode)
            .Where(x => x.OrderIndex >= startOrderIndex + temporaryOffset &&
                        x.OrderIndex <= endOrderIndex + temporaryOffset)
            .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(x => x.OrderIndex, x => x.OrderIndex - temporaryOffset + delta),
                cancellationToken);
    }

    private async Task<int?> GetMaxOrderIndex(string articleCode, CancellationToken cancellationToken)
    {
        return await context.ArticleImage
            .Where(x => x.ArticleCode == articleCode)
            .MaxAsync(x => (int?)x.OrderIndex, cancellationToken);
    }

    private async Task<int> GetTemporaryOffset(string articleCode, CancellationToken cancellationToken)
    {
        var maxOrderIndex = await GetMaxOrderIndex(articleCode, cancellationToken) ?? -1;
        return checked(maxOrderIndex + 2);
    }

    private async Task<int> GetTemporaryCurrentOrderIndex(string articleCode, CancellationToken cancellationToken)
    {
        var maxOrderIndex = await GetMaxOrderIndex(articleCode, cancellationToken) ?? -1;
        var temporaryOffset = checked(maxOrderIndex + 2);
        return checked(maxOrderIndex + temporaryOffset + 1);
    }
}