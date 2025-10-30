using Matterway.Ordering.Api.Features.Orders.Contracts;
using Matterway.Ordering.Api.Features.Orders.Domain;

namespace Matterway.Ordering.Api.Features.Orders.Data;

public interface IOrderRepository
{
    Task<ICollection<Order>> Query(int pageIndex, int pageSize);

    Task<Order?> GetById(Guid id);

    Task<Order?> Create(Order requestModel);

    Task<Order?> Update(Guid id, OrderUpdateRequest requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities(Guid systemUserId);

    Task<int> GetTotalEntities();

    Task<IEnumerable<Order>?> QueryByCustomerId(Guid systemUserId, int page, int pageSize);
}