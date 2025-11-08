using Microsoft.EntityFrameworkCore;
using DomainProductImage = Matterway.Catalog.Api.Domain.ProductImage;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;

public sealed class ProductImageRepository(CatalogDb context) : IProductImageRepository
{
    public async Task<DomainProductImage?> Create(DomainProductImage requestModel)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingCount = await context.ProductImage
                    .Where(pi => pi.ProductId == requestModel.ProductId)
                    .CountAsync();

                requestModel.Id = Math.Clamp(requestModel.Id, 0, existingCount);

                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"DomainProductImage\" SET \"Id\" = \"Id\" + 1 WHERE \"ProductId\" = {requestModel.ProductId} AND \"Id\" >= {requestModel.Id};");

                context.ProductImage.Add(requestModel);
                var affected = await context.SaveChangesAsync();
                if (affected != 1)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();

                return await context.ProductImage.Include(x => x.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == requestModel.Id && x.ProductId == requestModel.ProductId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<bool> Delete(Guid parentId, int id)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var affected = await context.ProductImage
                    .Where(model => model.ProductId == parentId)
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                if (affected != 1)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"DomainProductImage\" SET \"Id\" = \"Id\" - 1 WHERE \"ProductId\" = {parentId} AND \"Id\" > {id};");

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<DomainProductImage?> GetById(Guid parentId, int id)
    {
        return await context.ProductImage.FirstOrDefaultAsync(x => x.Id == id && x.ProductId == parentId);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.ProductImage.CountAsync();
    }

    public async Task<ICollection<DomainProductImage>> Query(int pageIndex, int pageSize)
    {
        return await context.ProductImage.AsNoTracking()
            .Include(x => x.Product)
            .OrderBy(x => x.ProductId)
            .ThenBy(x => x.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<DomainProductImage?> UpdateAsync(DomainProductImage request, int targetOrderIndex)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existing = await context.ProductImage
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.ImageRef == request.ImageRef);

                if (existing is null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var total = await context.ProductImage
                    .Where(x => x.ProductId == request.ProductId)
                    .CountAsync();

                if (total == 0)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var clampedTarget = Math.Clamp(targetOrderIndex, 0, total - 1);

                if (clampedTarget < existing.Id)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE \"DomainProductImage\" SET \"Id\" = \"Id\" + 1 WHERE \"ProductId\" = {request.ProductId} AND \"Id\" >= {clampedTarget} AND \"Id\" < {existing.Id};");
                }
                else if (clampedTarget > existing.Id)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE \"DomainProductImage\" SET \"Id\" = \"Id\" - 1 WHERE \"ProductId\" = {request.ProductId} AND \"Id\" > {existing.Id} AND \"Id\" <= {clampedTarget};");
                }

                var newAlt = request.ImageAlt ?? existing.ImageAlt;

                var updatedRows = await context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"DomainProductImage\" SET \"Id\" = {clampedTarget}, \"ImageAlt\" = {newAlt} WHERE \"ProductId\" = {request.ProductId} AND \"ImageRef\" = {request.ImageRef};");

                if (updatedRows != 1)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();

                return await context.ProductImage.Include(x => x.Product)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.ImageRef == request.ImageRef);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
