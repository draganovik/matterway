using Ordering.API.Entities;
using Ordering.API.Models.OrderItemModels;

namespace Ordering.API.Repository;

public interface IOrderItemRepository
{
    Task<ICollection<OrderItem>> Query();

    Task<OrderItem?> GetById(Guid id);

    Task<OrderItem?> Create(OrderItem requestModel);

    Task<OrderItem?> Update(Guid id, OrderItem requestModel);

    Task<bool> Delete(Guid id);
}
