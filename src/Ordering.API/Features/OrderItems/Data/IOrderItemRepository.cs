using Ordering.API.Features.OrderItems.Domain;

namespace Ordering.API.Features.OrderItems.Data;

public interface IOrderItemRepository
{
    Task<ICollection<OrderItem>> Query(int pageIndex, int pageSize);

    Task<OrderItem?> GetById(Guid orderId, Guid productId);

    Task<OrderItem?> Create(OrderItem requestModel);

    Task<OrderItem?> Put(OrderItem requestModel);

    Task<bool> Delete(Guid orderId, Guid productId);

    Task<int> GetTotalEntities(Guid systemUserId);

    Task<int> GetTotalEntities();
}