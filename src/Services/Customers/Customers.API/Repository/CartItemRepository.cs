using Customers.API.Data;
using Customers.API.Entities;
using Customers.API.Models.CartItemModels;
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
            return await context.CartItem.FindAsync(requestModel.Id);
        }
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.CartItem
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<CartItem?> GetById(Guid id)
    {
        return await context.CartItem.FindAsync(id);
    }

    public async Task<ICollection<CartItem>> Query()
    {
        return await context.CartItem.AsNoTracking()
        .ToListAsync();
    }

    public async Task<CartItem?> Update(Guid id, CartItemUpdateRequestModel requestModel)
    {
        var currentCartItemModel = await context.CartItem.FindAsync(id);
        if (currentCartItemModel is null)
        {
            return null;
        }
        var affected = await context.CartItem
        .Where(model => model.Id == id)
        .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.CustomerId, requestModel.CustomerId)
              .SetProperty(m => m.ProductId, requestModel.ProductId)
              .SetProperty(m => m.UnitPrice, requestModel.UnitPrice)
              .SetProperty(m => m.ProductName, requestModel.ProductName)
            );
        await context.Entry(currentCartItemModel).ReloadAsync();
        return affected == 1 ? currentCartItemModel : null;
    }
}
