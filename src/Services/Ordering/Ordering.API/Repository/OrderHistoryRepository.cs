using Microsoft.EntityFrameworkCore;
using Ordering.API.Data;
using Ordering.API.Entities;
using Ordering.API.Models.OrderHistoryModels;

namespace Ordering.API.Repository;

public class OrderHistoryRepository : IOrderHistoryRepository
{
    private readonly OrderingDbContext context;

    public OrderHistoryRepository(OrderingDbContext context)
    {
        this.context = context;
    }

    public async Task<OrderHistory?> Create(OrderHistory requestModel)
    {
        context.OrderHistory.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.OrderHistory.FindAsync(requestModel.Id);
        }
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.OrderHistory
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<OrderHistory?> GetById(Guid id)
    {
        return await context.OrderHistory.FindAsync(id);
    }

    public async Task<ICollection<OrderHistory>> Query(int pageIndex, int pageSize)
    {
        return await context.OrderHistory.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<OrderHistory?> Update(Guid id, OrderHistoryBaseRequestModel requestModel)
    {
        var currentOrderHistoryModel = await context.OrderHistory.FindAsync(id);
        if (currentOrderHistoryModel is null)
        {
            return null;
        }
        var affected = await context.OrderHistory
        .Where(model => model.Id == id)
        .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.OrderId, requestModel.OrderId)
              .SetProperty(m => m.OrderStatus, requestModel.OrderStatus)
              .SetProperty(m => m.Description, requestModel.Description)
            );
        await context.Entry(currentOrderHistoryModel).ReloadAsync();
        return affected == 1 ? currentOrderHistoryModel : null;
    }
}
