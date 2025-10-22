using Microsoft.EntityFrameworkCore;
using Ordering.Api.Data;
using Ordering.Api.Features.Orders.Contracts;
using Ordering.Api.Features.Orders.Domain;

namespace Ordering.Api.Features.Orders.Data;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext context;

    public OrderRepository(OrderingDbContext context)
    {
        this.context = context;
    }

    public async Task<Order?> Create(Order requestModel)
    {
        context.Order.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.Order
                .Include(o => o.OrderHistory)
                .Include(o => o.OrderItems)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == requestModel.Id);
        }

        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.Order
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<Order?> GetById(Guid id)
    {
        return await context.Order
            .Include(o => o.OrderHistory)
            .Include(o => o.OrderItems)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<int> GetTotalEntities(Guid systemUserId)
    {
        return await context.Order
            .CountAsync(model => model.CustomerId == systemUserId);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.Order.CountAsync();
    }

    public async Task<ICollection<Order>> Query(int pageIndex, int pageSize)
    {
        return await context.Order
            .Include(o => o.OrderHistory)
            .Include(o => o.OrderItems)
            .Include(o => o.Address)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>?> QueryByCustomerId(Guid systemUserId, int page, int pageSize)
    {
        return await context.Order
            .Include(o => o.OrderHistory)
            .Include(o => o.OrderItems)
            .Include(o => o.Address)
            .AsNoTracking()
            .Where(model => model.CustomerId == systemUserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Order?> Update(Guid id, OrderUpdateRequest requestModel)
    {
        var currentOrderModel = await context.Order
            .Include(o => o.OrderHistory)
            .Include(o => o.OrderItems)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (currentOrderModel is null) return null;

        var affected = await context.Order
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.CustomerId, requestModel.CustomerId)
                .SetProperty(m => m.DeliveryAddressId, requestModel.DeliveryAddressId)
                .SetProperty(m => m.ReferenceNumber, requestModel.ReferenceNumber)
            );

        await context.Entry(currentOrderModel).ReloadAsync();
        return affected == 1 ? currentOrderModel : null;
    }
}