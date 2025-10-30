using Matterway.Ordering.Api.Data;
using Matterway.Ordering.Api.Features.OrderItems.Domain;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Ordering.Api.Features.OrderItems.Data;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly OrderingDb context;

    public OrderItemRepository(OrderingDb context)
    {
        this.context = context;
    }

    public async Task<OrderItem?> Create(OrderItem requestModel)
    {
        context.OrderItem.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
            return await context.OrderItem.FirstOrDefaultAsync(x =>
                x.ProductId == requestModel.ProductId && x.OrderId == requestModel.OrderId);
        return null;
    }

    public async Task<bool> Delete(Guid orderId, Guid productId)
    {
        var affected = await context.OrderItem
            .Where(model => model.ProductId == productId)
            .Where(model => model.OrderId == orderId)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<OrderItem?> GetById(Guid orderId, Guid productId)
    {
        return await context.OrderItem.FirstOrDefaultAsync(x =>
            x.ProductId == productId && x.OrderId == orderId);
    }

    public async Task<ICollection<OrderItem>> Query(int pageIndex, int pageSize)
    {
        return await context.OrderItem.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<OrderItem?> Put(OrderItem requestModel)
    {
        var currentOrderItemModel = await context.OrderItem.FirstOrDefaultAsync(x =>
            x.ProductId == requestModel.ProductId && x.OrderId == requestModel.OrderId);
        if (currentOrderItemModel is null)
        {
            context.OrderItem.Add(requestModel);
            var inserted = await context.SaveChangesAsync();
            if (inserted == 1)
            {
                return await context.OrderItem.FirstOrDefaultAsync(x =>
                    x.ProductId == requestModel.ProductId && x.OrderId == requestModel.OrderId);
            }

            return null;
        }

        var affected = await context.OrderItem
            .Where(model => model.ProductId == requestModel.ProductId)
            .Where(model => model.OrderId == requestModel.OrderId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.OrderId, requestModel.OrderId)
                .SetProperty(m => m.ProductId, requestModel.ProductId)
                .SetProperty(m => m.ProductName, requestModel.ProductName)
                .SetProperty(m => m.UnitPrice, requestModel.UnitPrice)
                .SetProperty(m => m.Quantity, requestModel.Quantity)
            );
        await context.Entry(currentOrderItemModel).ReloadAsync();
        return affected == 1 ? currentOrderItemModel : null;
    }

    public async Task<int> GetTotalEntities(Guid systemUserId)
    {
        return await context.OrderItem.CountAsync(model =>
            model.Order != null && model.Order.CustomerId == systemUserId);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.OrderItem.CountAsync();
    }
}