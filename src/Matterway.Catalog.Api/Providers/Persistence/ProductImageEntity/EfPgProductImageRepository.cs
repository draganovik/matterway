using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductImageEntity;

public sealed class EfPgProductImageRepository(CatalogDbComposer context) : IProductImageRepository
{
    public async Task<ProductImage?> Create(ProductImage requestModel, CancellationToken cancellationToken)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existingCount = await context.ProductImage
                    .Where(pi => pi.ProductId == requestModel.ProductId)
                    .CountAsync(cancellationToken);

                requestModel.OrderIndex = Math.Clamp(requestModel.OrderIndex, 0, existingCount);

                await context.ProductImage
                    .Where(pi => pi.ProductId == requestModel.ProductId)
                    .Where(pi => pi.OrderIndex >= requestModel.OrderIndex)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(pi => pi.OrderIndex, pi => pi.OrderIndex + 1), cancellationToken);

                context.ProductImage.Add(requestModel);
                var affected = await context.SaveChangesAsync(cancellationToken);
                if (affected != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                await transaction.CommitAsync(cancellationToken);

                return await context.ProductImage.Include(x => x.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == requestModel.Id && x.ProductId == requestModel.ProductId,
                        cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<bool> Delete(Guid parentId, int orderIndex, CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var affected = await context.ProductImage
                    .Where(model => model.ProductId == parentId)
                    .Where(model => model.OrderIndex == orderIndex)
                    .ExecuteDeleteAsync(cancellationToken);
                if (affected != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                await context.ProductImage
                    .Where(model => model.ProductId == parentId)
                    .Where(model => model.OrderIndex > orderIndex)
                    .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(model => model.OrderIndex, model => model.OrderIndex - 1),
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

    public async Task<ProductImage?> GetBy(Guid parentId, int orderIndex,
        CancellationToken cancellationToken = default)
    {
        return await
            context.ProductImage.FirstOrDefaultAsync(x => x.OrderIndex == orderIndex && x.ProductId == parentId,
                cancellationToken);
    }

    public async Task<ProductImage?> Update(ProductImage request, int targetOrderIndex,
        CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existing = await context.ProductImage
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.Id == request.Id,
                        cancellationToken);

                if (existing is null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                var total = await context.ProductImage
                    .Where(x => x.ProductId == request.ProductId)
                    .CountAsync(cancellationToken);

                if (total == 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                var clampedTarget = Math.Clamp(targetOrderIndex, 0, total - 1);

                if (clampedTarget < existing.OrderIndex)
                    await context.ProductImage
                        .Where(x => x.ProductId == request.ProductId)
                        .Where(x => x.OrderIndex >= clampedTarget && x.OrderIndex < existing.OrderIndex)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(x => x.OrderIndex, x => x.OrderIndex + 1), cancellationToken);
                else if (clampedTarget > existing.OrderIndex)
                    await context.ProductImage
                        .Where(x => x.ProductId == request.ProductId)
                        .Where(x => x.OrderIndex > existing.OrderIndex && x.OrderIndex <= clampedTarget)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(x => x.OrderIndex, x => x.OrderIndex - 1), cancellationToken);

                var newAlt = request.ImageAlt ?? existing.ImageAlt;

                var updatedRows = await context.ProductImage
                    .Where(x => x.ProductId == request.ProductId && x.Id == request.Id)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(x => x.OrderIndex, x => clampedTarget)
                            .SetProperty(x => x.ImageAlt, x => newAlt), cancellationToken);

                if (updatedRows != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                await transaction.CommitAsync(cancellationToken);

                return await context.ProductImage.Include(x => x.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.Id == request.Id,
                        cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}