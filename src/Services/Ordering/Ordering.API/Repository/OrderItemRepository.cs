using Microsoft.EntityFrameworkCore;
using Ordering.API.Data;
using Ordering.API.Entities;

namespace Ordering.API.Repository;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly OrderingDbContext context;

    public OrderItemRepository(OrderingDbContext context)
    {
        this.context = context;
    }

    public async Task<OrderItem?> Create(OrderItem requestModel)
    {
        context.OrderItem.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.OrderItem.FindAsync(requestModel.Id);
        }
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.OrderItem
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<OrderItem?> GetById(Guid id)
    {
        return await context.OrderItem.FindAsync(id);
    }

    public async Task<ICollection<OrderItem>> Query()
    {
        return await context.OrderItem.AsNoTracking()
        .ToListAsync();
    }

    public async Task<OrderItem?> Update(Guid id, OrderItem requestModel)
    {
        var currentOrderItemModel = await context.OrderItem.FindAsync(id);
        if (currentOrderItemModel is null)
        {
            return null;
        }
        var affected = await context.OrderItem
        .Where(model => model.Id == id)
        .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.Id, requestModel.Id)
                  .SetProperty(m => m.OrderId, requestModel.OrderId)
                  .SetProperty(m => m.ProductId, requestModel.ProductId)
                  .SetProperty(m => m.ProductName, requestModel.ProductName)
                  .SetProperty(m => m.UnitPrice, requestModel.UnitPrice)
                  .SetProperty(m => m.Units, requestModel.Units)
            );
        await context.Entry(currentOrderItemModel).ReloadAsync();
        return affected == 1 ? currentOrderItemModel : null;
    }
}
