using Ordering.API.Entities;
using Ordering.API.Models.OrderHistoryModels;

namespace Ordering.API.Repository;

public interface IOrderHistoryRepository
{
    Task<ICollection<OrderHistory>> Query(int pageIndex, int pageSize);

    Task<OrderHistory?> GetById(Guid id);

    Task<OrderHistory?> Create(OrderHistory requestModel);

    Task<OrderHistory?> Update(Guid id, OrderHistoryBaseRequestModel requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}