using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

public class EfPgCartItemRepository(CustomersDb context) : ICartItemRepository
{
    public async Task<ICollection<CartItem>> QueryForSuid(Guid systemUserId, int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.CartItem.Where(x => x.Customer != null && x.Customer.SystemUserId == systemUserId)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<CartItem?> Upsert(CartItem requestModel, CancellationToken cancellationToken = default)
    {
        var existing = await context.CartItem
            .FirstOrDefaultAsync(x => x.CustomerId == requestModel.CustomerId && x.ProductId == requestModel.ProductId,
                cancellationToken);

        if (existing != null)
            context.CartItem.Entry(existing).CurrentValues.SetValues(requestModel);
        else
            context.CartItem.Add(requestModel);

        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0 ? existing ?? requestModel : null;
    }

    public async Task<bool> Delete(Guid id, Guid productId, CancellationToken cancellationToken = default)
    {
        var affected = await context.CartItem
            .Where(model => model.CustomerId == id)
            .Where(model => model.ProductId == productId)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<CartItem?> GetBy(Guid id, Guid productId, CancellationToken cancellationToken = default)
    {
        return await context.CartItem.FirstOrDefaultAsync(x => x.CustomerId == id && x.ProductId == productId,
            cancellationToken);
    }

    public async Task<int> Count(Guid systemUserId, CancellationToken cancellationToken = default)
    {
        return await context.CartItem.Where(x => x.Customer != null && x.Customer.SystemUserId == systemUserId)
            .CountAsync(cancellationToken);
    }
}