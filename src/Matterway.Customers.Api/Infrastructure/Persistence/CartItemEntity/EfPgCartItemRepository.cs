using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

public class EfPgCartItemRepository(CustomersDb context) : ICartItemRepository
{
    public async Task<ICollection<CartItem>> QueryForSystemUserId(Guid systemUserId, int pageIndex, int pageSize)
    {
        return await context.CartItem.Where(x => x.Customer != null && x.Customer.SystemUserId == systemUserId)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<CartItem?> Upsert(CartItem requestModel)
    {
        var existing = await context.CartItem
            .FirstOrDefaultAsync(x => x.CustomerId == requestModel.CustomerId && x.ProductId == requestModel.ProductId);

        if (existing != null)
            context.CartItem.Entry(existing).CurrentValues.SetValues(requestModel);
        else
            context.CartItem.Add(requestModel);

        var affected = await context.SaveChangesAsync();
        return affected > 0 ? existing ?? requestModel : null;
    }

    public async Task<bool> Delete(Guid id, Guid productId)
    {
        var affected = await context.CartItem
            .Where(model => model.CustomerId == id)
            .Where(model => model.ProductId == productId)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<CartItem?> GetById(Guid id, Guid productId)
    {
        return await context.CartItem.FirstOrDefaultAsync(x => x.CustomerId == id && x.ProductId == productId);
    }

    public async Task<int> Count(Guid systemUserId)
    {
        return await context.CartItem.Where(x => x.Customer != null && x.Customer.SystemUserId == systemUserId)
            .CountAsync();
    }
}