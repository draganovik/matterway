using Ordering.API.Entities;
using Ordering.API.Models.OrderModels;

namespace Ordering.API.Repository;

public interface IOrderRepository
{
    Task<ICollection<Order>> Query(int pageIndex, int pageSize);

    Task<Order?> GetById(Guid id);

    Task<Order?> Create(Order requestModel);

    Task<Order?> Update(Guid id, OrderUpdateRequestModel requestModel);

    Task<bool> Delete(Guid id);
    Task<int> GetTotalEntities(Guid systemUserId);

    Task<int> GetTotalEntities();
}
