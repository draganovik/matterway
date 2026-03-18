using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;

public class EfPgCustomerOrderRepository(CustomersDbComposer context) : ICustomerOrderRepository
{
    public async Task<CustomerOrder?> CreateFromCart(Guid customerId, OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        var existing = await context.CustomerOrder
            .Include(order => order.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.OrderId == orderId, cancellationToken);
        if (existing is not null) return existing;

        var cartItems = await context.CustomerArticle
            .Where(article => article.CustomerId == customerId && article.OrderId == null)
            .ToListAsync(cancellationToken);
        if (cartItems.Count == 0) return null;

        var order = new CustomerOrder
        {
            OrderId = orderId,
            CustomerId = customerId,
            PlacedAt = DateTime.UtcNow
        };

        context.CustomerOrder.Add(order);
        foreach (var item in cartItems)
            item.OrderId = orderId;

        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected < 1) return null;

        return await context.CustomerOrder
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);
    }

    public async Task<ICollection<CustomerOrder>> QueryForCustomer(Guid customerId, int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.CustomerOrder
            .Where(order => order.CustomerId == customerId)
            .Include(order => order.Items)
            .AsNoTracking()
            .OrderByDescending(order => order.PlacedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountForCustomer(Guid customerId, CancellationToken cancellationToken = default)
    {
        return context.CustomerOrder
            .Where(order => order.CustomerId == customerId)
            .CountAsync(cancellationToken);
    }
}