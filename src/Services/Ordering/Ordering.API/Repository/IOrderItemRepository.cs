using Ordering.API.Entities;

namespace Ordering.API.Repository;

public interface IOrderItemRepository
{
    Task<ICollection<OrderItem>> Query();

    Task<OrderItem?> GetById(Guid orderId, Guid productId);

    Task<OrderItem?> Create(OrderItem requestModel);

    Task<OrderItem?> Put(OrderItem requestModel);

    Task<bool> Delete(Guid orderId, Guid productId);
}
