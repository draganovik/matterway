using Customers.API.Data;
using Customers.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Repository;

public class CartItemRepository : ICartItemRepository
{
    private readonly CustomersDbContext context;

    public CartItemRepository(CustomersDbContext context)
    {
        this.context = context;
    }

    public async Task<CartItem?> Create(CartItem requestModel)
    {
        context.CartItem.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.CartItem.FirstOrDefaultAsync(x => x.CustomerId == requestModel.CustomerId && x.ProductId == requestModel.ProductId);
        }
        return null;
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

    public async Task<ICollection<CartItem>> Query(int pageIndex, int pageSize)
    {
        return await context.CartItem.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<CartItem?> Put(CartItem requestModel)
    {
        var currentCartItemModel = await context.CartItem.FirstOrDefaultAsync(x => x.CustomerId == requestModel.CustomerId && x.ProductId == requestModel.ProductId);
        var affected = 0;
        if (currentCartItemModel is null)
        {
            context.CartItem.Add(requestModel);
            affected = await context.SaveChangesAsync();
            if (affected == 1)
            {
                return await context.CartItem.FirstOrDefaultAsync(x => x.CustomerId == requestModel.CustomerId && x.ProductId == requestModel.ProductId);
            }
            return null;
        }
        affected = await context.CartItem
            .Where(model => model.CustomerId == currentCartItemModel.CustomerId)
            .Where(model => model.ProductId == currentCartItemModel.ProductId)
        .ExecuteUpdateAsync(setters => setters
               .SetProperty(m => m.CustomerId, requestModel.CustomerId)
               .SetProperty(m => m.ProductId, requestModel.ProductId)
               .SetProperty(m => m.UnitPrice, requestModel.UnitPrice)
               .SetProperty(m => m.Quantity, requestModel.Quantity)
               .SetProperty(m => m.ProductName, requestModel.ProductName)
            );
        await context.Entry(currentCartItemModel).ReloadAsync();
        return affected == 1 ? currentCartItemModel : null;
    }

    public async Task<ICollection<CartItem>> QueryByCustomerId(Guid systemUserId, int pageIndex, int pageSize)
    {
        return await context.CartItem.Where(x => x.Customer != null && x.Customer.SystemUserId == systemUserId)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalEntities(Guid systemUserId)
    {
        return await context.CartItem.Where(x => x.Customer != null && x.Customer.SystemUserId == systemUserId).CountAsync();
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.CartItem.CountAsync();
    }
}
