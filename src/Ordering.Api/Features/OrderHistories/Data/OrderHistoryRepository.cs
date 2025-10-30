using Microsoft.EntityFrameworkCore;
using Ordering.Api.Data;
using Ordering.Api.Features.OrderHistories.Contracts;
using Ordering.Api.Features.OrderHistories.Domain;

namespace Ordering.Api.Features.OrderHistories.Data;

public class OrderHistoryRepository : IOrderHistoryRepository
{
    private readonly OrderingDb context;

    public OrderHistoryRepository(OrderingDb context)
    {
        this.context = context;
    }

    public async Task<OrderHistory?> Create(OrderHistory requestModel)
    {
        context.OrderHistory.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.OrderHistory.FindAsync(requestModel.Id);
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

    public async Task<int> GetTotalEntities()
    {
        return await context.OrderHistory.CountAsync();
    }

    public async Task<ICollection<OrderHistory>> Query(int pageIndex, int pageSize)
    {
        return await context.OrderHistory.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<OrderHistory?> Update(Guid id, OrderHistoryBaseRequest requestModel)
    {
        var currentOrderHistoryModel = await context.OrderHistory.FindAsync(id);
        if (currentOrderHistoryModel is null) return null;
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