using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

public sealed class EfPgOrderRepository(SalesDbComposer context) : IOrderRepository
{
    public async Task<Order?> Create(Order requestModel, CancellationToken cancellationToken = default)
    {
        context.Order.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await QueryWithDetails()
                .FirstOrDefaultAsync(order => order.Id == requestModel.Id, cancellationToken);

        return null;
    }

    public async Task<Order?> Update(Order requestModel, CancellationToken cancellationToken = default)
    {
        var existing = await QueryWithDetails()
            .FirstOrDefaultAsync(order => order.Id == requestModel.Id, cancellationToken);
        if (existing is null) return null;

        existing.CustomerId = requestModel.CustomerId;
        existing.Type = requestModel.Type;

        if (existing.DeliveryInfo is not null)
        {
            context.OrderDeliveryInfo.Remove(existing.DeliveryInfo);
            existing.DeliveryInfo = null;
        }

        if (existing.Items.Count > 0)
        {
            context.OrderItem.RemoveRange(existing.Items);
            existing.Items.Clear();
        }

        if (requestModel.DeliveryInfo is not null)
            context.OrderDeliveryInfo.Add(requestModel.DeliveryInfo);

        if (requestModel.Items.Count > 0)
            context.OrderItem.AddRange(requestModel.Items);

        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await QueryWithDetails()
                .FirstOrDefaultAsync(order => order.Id == requestModel.Id, cancellationToken);

        return null;
    }

    public async Task<Order?> GetById(OrderId id, CancellationToken cancellationToken = default)
    {
        return await QueryWithDetails()
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<Order?> GetPendingByCustomerId(Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await context.Order
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId &&
                            !order.Items.Any() &&
                            !order.Payments.Any())
            .OrderByDescending(order => order.PlacedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ICollection<Order>> Query(int pageIndex, int pageSize, Guid? customerId,
        CancellationToken cancellationToken = default)
    {
        var query = QueryWithDetails()
            .AsNoTracking();

        if (customerId.HasValue)
            query = query.Where(order => order.CustomerId == customerId);

        return await query
            .OrderByDescending(order => order.PlacedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> Count(Guid? customerId, CancellationToken cancellationToken = default)
    {
        var query = context.Order.AsQueryable();
        if (customerId.HasValue)
            query = query.Where(order => order.CustomerId == customerId);

        return await query.CountAsync(cancellationToken);
    }

    public Task<bool> Exists(OrderId id, CancellationToken cancellationToken = default)
    {
        return context.Order.AnyAsync(order => order.Id == id, cancellationToken);
    }

    private IQueryable<Order> QueryWithDetails()
    {
        return context.Order
            .Include(order => order.DeliveryInfo)
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .Include(order => order.Payments);
    }
}